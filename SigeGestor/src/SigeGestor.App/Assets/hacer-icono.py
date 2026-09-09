# -*- coding: utf-8 -*-
"""
Genera SigeGestor.ico a partir del PNG de la marca.

POR QUÉ ESTE SCRIPT Y NO EL PNG DIRECTO
=======================================
El PNG original lleva el texto «Gestor de Datos SIGE» dentro del dibujo. A 48 px se lee; a
32, 24 y 16 —barra de título, Alt+Tab, listas del explorador— se convierte en un borrón azul
sin forma. Y esos tamaños pequeños son los que más se ven.

Así que el .ico lleva DOS dibujos, que es lo que hace Windows con sus propios iconos:

    16, 20, 24, 32 px  ->  un cilindro de base de datos dibujado a mano, sin texto
    40, 48, 64, 128, 256  ->  el PNG original recortado

El corte está en 40 px (CORTE). Los pequeños van en DIB, el formato clásico del .ico, y no en
PNG: el explorador de Windows entiende PNG a cualquier tamaño, pero hay rincones antiguos del
shell que esperan DIB en 16 y 32, y no cuesta nada dárselo.

Se escribe el contenedor .ico a mano porque Pillow solo sabe reducir UNA imagen a todos los
tamaños, y aquí hacen falta dos.

CÓMO SE USA
===========
    pip install pillow
    python hacer-icono.py [ruta-del-png]

Sin argumento usa ORIGEN. Deja el .ico en esta misma carpeta; el .csproj lo recoge con
<ApplicationIcon> y de ahí sale el icono del ejecutable, de la barra de tareas y de las
ventanas. No hay que copiar nada al servidor: va dentro del .exe.
"""
import io
import os
import struct
import sys

from PIL import Image, ImageDraw

AQUI = os.path.dirname(os.path.abspath(__file__))
ORIGEN = r"C:\Users\ErickCC\Desktop\Erick\ConsultasWindsurf\IconoGestordeDatosSIGE.png"
DESTINO = os.path.join(AQUI, "SigeGestor.ico")

TAMANOS = [16, 20, 24, 32, 40, 48, 64, 128, 256]
CORTE = 40  # por debajo de esto, el dibujo simple

# Muestreados del PNG original: azul profundo arriba-izquierda, más claro abajo-derecha.
AZUL_A = (0x12, 0x5C, 0xD8)
AZUL_B = (0x00, 0x37, 0xA6)
BLANCO = (0xFF, 0xFF, 0xFF, 0xFF)


# ---------------------------------------------------------------- la imagen real
def imagen_real(ruta):
    """El PNG sin el margen transparente y cuadrado, listo para reducir."""
    im = Image.open(ruta).convert("RGBA")

    # El original trae un 25 % de margen vacío alrededor. Sin recortarlo, el icono se ve
    # pequeño y perdido en la barra de tareas al lado de los demás.
    im = im.crop(im.split()[3].getbbox())

    # A cuadrado centrado, sin deformar: el contenido no era cuadrado (1095x1068).
    lado = max(im.size)
    lienzo = Image.new("RGBA", (lado, lado), (0, 0, 0, 0))
    lienzo.paste(im, ((lado - im.width) // 2, (lado - im.height) // 2))
    return lienzo


# ------------------------------------------------------------- el dibujo simple
def dibujo_simple(lado=256, sup=4):
    """Cuadrado azul redondeado con un cilindro de base de datos. Se dibuja a 4x y se
    reduce, que es lo que suaviza los bordes."""
    L = lado * sup
    im = Image.new("RGBA", (L, L), (0, 0, 0, 0))

    # Degradado diagonal, del azul profundo al más claro. De 8 en 8 columnas y se rellena:
    # pixel a pixel sobre 1024x4 tarda un minuto largo y no se nota la diferencia.
    grad = Image.new("RGBA", (L, L))
    px = grad.load()
    for y in range(L):
        for x in range(0, L, 8):
            t = (x + y) / (2 * L - 2)
            c = (int(AZUL_A[0] + (AZUL_B[0] - AZUL_A[0]) * t),
                 int(AZUL_A[1] + (AZUL_B[1] - AZUL_A[1]) * t),
                 int(AZUL_A[2] + (AZUL_B[2] - AZUL_A[2]) * t), 255)
            for d in range(min(8, L - x)):
                px[x + d, y] = c

    # Recortado al cuadrado redondeado, con el mismo radio que el original (~22 %).
    mascara = Image.new("L", (L, L), 0)
    ImageDraw.Draw(mascara).rounded_rectangle(
        [0, 0, L - 1, L - 1], radius=int(L * 0.225), fill=255)
    im.paste(grad, (0, 0), mascara)

    d = ImageDraw.Draw(im)

    # El cilindro. Las rodajas NO se solapan: el hueco entre ellas deja ver el azul del
    # fondo, y es eso lo que hace que se lea como «base de datos». Solapándolas, al ser
    # todas blancas, se fusionan en una mancha.
    An = L * 0.60                # ancho del cilindro
    ey = An * 0.26               # alto de la elipse: la perspectiva
    cuerpo = An * 0.085          # tramo recto de cada rodaja
    hueco = An * 0.065           # el azul que se ve entre rodajas
    rodaja = cuerpo + ey
    x0 = (L - An) / 2
    y0 = (L - (3 * rodaja + 2 * hueco)) / 2

    for i in range(3):
        arriba = y0 + i * (rodaja + hueco)
        d.ellipse([x0, arriba, x0 + An, arriba + ey], fill=BLANCO)
        d.rectangle([x0, arriba + ey / 2, x0 + An, arriba + cuerpo + ey / 2], fill=BLANCO)
        d.ellipse([x0, arriba + cuerpo, x0 + An, arriba + cuerpo + ey], fill=BLANCO)

    return im.resize((lado, lado), Image.LANCZOS)


# --------------------------------------------------------- el contenedor .ico
def como_dib(im):
    """BITMAPINFOHEADER + píxeles BGRA de abajo arriba + máscara AND vacía.

    El alto del encabezado va DOBLE (s * 2) porque el formato cuenta la imagen y la máscara
    como una sola. Con 32 bits la máscara la ignora Windows —manda el canal alfa— pero tiene
    que estar y ocupar su sitio."""
    a, s = im.width, im.height
    cab = struct.pack("<IiiHHIIiiII", 40, a, s * 2, 1, 32, 0, 0, 0, 0, 0, 0)
    px = im.load()
    xor = bytearray()
    for y in range(s - 1, -1, -1):
        for x in range(a):
            r, g, b, al = px[x, y]
            xor += bytes((b, g, r, al))
    fila = ((a + 31) // 32) * 4      # 1 bpp, filas a múltiplo de 4 bytes
    return bytes(cab) + bytes(xor) + bytes(fila * s)


def como_png(im):
    b = io.BytesIO()
    im.save(b, format="PNG", optimize=True)
    return b.getvalue()


def escribir(ruta, laminas):
    cabecera = struct.pack("<HHH", 0, 1, len(laminas))
    desplazamiento = 6 + 16 * len(laminas)
    entradas, cuerpo = b"", b""
    for lado, datos in laminas:
        d = 0 if lado == 256 else lado      # en el .ico, 0 significa 256
        entradas += struct.pack("<BBBBHHII", d, d, 0, 0, 1, 32, len(datos), desplazamiento)
        cuerpo += datos
        desplazamiento += len(datos)
    with open(ruta, "wb") as f:
        f.write(cabecera + entradas + cuerpo)


def main():
    origen = sys.argv[1] if len(sys.argv) > 1 else ORIGEN
    if not os.path.exists(origen):
        print(f"No encuentro el PNG: {origen}")
        return 1

    grande = imagen_real(origen)
    chico = dibujo_simple(256)

    laminas = []
    for t in TAMANOS:
        if t < CORTE:
            laminas.append((t, como_dib(chico.resize((t, t), Image.LANCZOS))))
        else:
            laminas.append((t, como_png(grande.resize((t, t), Image.LANCZOS))))

    escribir(DESTINO, laminas)

    # Se relee: si el contenedor estuviera mal montado, aquí reventaría en vez de dar un
    # icono en blanco meses después.
    v = Image.open(DESTINO)
    assert sorted(v.ico.sizes()) == sorted((t, t) for t in TAMANOS), "faltan tamaños"
    for t in TAMANOS:
        lam = v.ico.getimage((t, t)).convert("RGBA")
        assert lam.size == (t, t), f"{t} px salió {lam.size}"
        assert lam.split()[3].getextrema()[0] == 0, f"{t} px sin transparencia"

    print(f"{DESTINO}  ({os.path.getsize(DESTINO):,} bytes)")
    for t in TAMANOS:
        print(f"   {t:>3} px  {'dibujo simple (DIB)' if t < CORTE else 'imagen real (PNG)'}")
    print("\nlas 9 láminas se releen bien, del tamaño correcto y con transparencia")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
