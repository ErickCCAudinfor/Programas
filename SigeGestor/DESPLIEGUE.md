# Desplegar SigeGestor

El `.exe` vive en el servidor **172.31.100.13** y las ocho personas del BPO lo abren desde ahí.
No se instala nada en cada máquina: se copia una carpeta y se abre por red, igual que hoy con
ActualizaPrecios.

```powershell
.\publicar.ps1 -Destino \\172.31.100.13\Total\SigeGestor
```

Sin `-Destino` solo publica en local, en `publish\win-x64`, para poder revisarlo antes de tocar
el servidor.

---

## Lo que NUNCA se pisa en el servidor

| Qué | Por qué |
|---|---|
| `Config\Entornos.json` | credenciales de Producción, Réplica y UAT |
| `Config\Empresas.json` | bases de clientes, con credenciales |
| `Config\Usuarios.json` | qué novedades ha visto cada uno |
| `Logs\` | el histórico de ejecuciones del equipo, tres días |

Está protegido en **dos sitios a la vez**, y es a propósito:

1. **En el proyecto** — esos tres JSON se copian solo en Debug y llevan
   `CopyToPublishDirectory="Never"`, así que la carpeta de publicación sale sin ellos. Sin esto,
   publicar desde una máquina de desarrollo metería el `Entornos.json` de quien publica y, al
   copiar, el servidor se quedaría con su configuración.
2. **En el guion** — `robocopy /MIR` deja el destino idéntico al origen, o sea que **borraría**
   la configuración y los logs del servidor. Por eso van excluidos con `/XF` y `/XD`. La
   exclusión se repite aunque el csproj ya los quite: si algún día alguien los vuelve a incluir
   en el proyecto, esto sigue protegiendo al servidor.

El guion comprueba las dos cosas y avisa si algo se ha colado.

---

## Por qué autocontenido

Se publica con `--self-contained true`, o sea con el runtime de .NET dentro. Así las máquinas
del equipo no necesitan tener instalado el **.NET 8 Desktop Runtime**, que es lo que haría falta
si se publicara dependiente del framework.

Es lo mismo que se hace hoy con ActualizaPrecios, que publica `win-x86` autocontenido. El precio
son unos **180 MB** de carpeta. Al abrirlo por red Windows no trae el paquete entero, solo lo que
va necesitando, pero el primer arranque del día se nota.

Si algún día se confirma que las ocho máquinas tienen el runtime 8 instalado, quitando
`--self-contained` la carpeta baja a unos 15 MB.

---

## La primera vez

En el servidor no habrá configuración, así que después de copiar hay que crearla:

1. `Config\Entornos.json` — a partir de `Entornos.ejemplo.json`. El usuario y la contraseña van
   **cifrados**: sirven tal cual los del `EmpresasBD.json` de ActualizaPrecios, porque es el
   mismo AES.
2. `Config\Empresas.json` — se puede copiar directamente el `Json\EmpresasBD.json` de
   ActualizaPrecios. Mismo formato y mismo cifrado.
3. `Config\Usuarios.json` y `Logs\` no hay que crearlos: se crean solos al usarse.

El guion detecta si es la primera vez y lo recuerda.

---

## Comprobar que ha ido bien

Al terminar, el guion lista si los tres JSON siguen en el servidor. Además, dentro de la
aplicación:

- **Ajustes → Empresas y bases de datos** → «Probar todas». Si conectan, las credenciales han
  sobrevivido a la copia.
- La ruta del fichero de empresas se muestra en esa misma pantalla: tiene que apuntar al `.13`,
  no al disco local de quien lo abre.
- **Ejecuciones** debe seguir mostrando el histórico anterior al despliegue.

---

## Volver atrás

No hay versionado: `/MIR` deja una sola copia. Si hace falta poder volver, lo más simple es
renombrar la carpeta del servidor antes de copiar (`SigeGestor_anterior`) y borrarla cuando la
nueva lleve unos días sin quejas.

**Ojo con eso:** la configuración y los logs viven **dentro** de esa carpeta, así que al
renombrarla se van con ella. Hay que copiar a mano `Config\` y `Logs\` de la carpeta vieja a la
nueva, o el equipo se encontrará una aplicación sin entornos configurados y sin histórico.

Es el motivo por el que la vía normal es copiar encima con las exclusiones y no renombrar: así la
configuración y los logs no se mueven nunca de sitio.
