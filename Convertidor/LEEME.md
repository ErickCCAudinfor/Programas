# Convertidor

Pasa archivos de un formato a otro. Se arrastran a la ventana, o encima de `Convertidor.exe`, se elige el formato y listo.

## Cómo se usa

1. Arrastra uno o varios archivos (o carpetas enteras) a la ventana, o suéltalos encima del `.exe` o de un acceso directo.
2. En **Convertir a** salen los formatos posibles. Si alguno no vale para todos los archivos de la lista, se indica para cuántos (`PDF · 2 de 3`).
3. Elige dónde guardar: junto a cada original (lo normal) o en otra carpeta.
4. **Convertir**. El original no se toca nunca y tampoco se pisa nada: si `informe.pdf` ya existe, el nuevo se llama `informe (1).pdf`.
5. Con doble clic en un archivo convertido se abre su carpeta con el resultado seleccionado.

Atajos: `Supr` quita de la lista los archivos seleccionados.

## Recortar: sacar una zona de un PDF o de una imagen

Selecciona un PDF o una imagen en la lista y pulsa **Recortar…** (o clic derecho → *Recortar / extraer una zona…*). Se abre una ventana con la página:

- **Marcar la zona**: arrastra sobre la página. Las asas de las esquinas y los lados la ajustan, y arrastrando por dentro se mueve. Un clic fuera, o `Esc`, la quita (sin zona se guarda la página entera).
- **Ajustar al contenido**: recorta los márgenes en blanco de la zona, o de la página. Viene bien para quedarse justo con una foto o un logo.
- **Páginas**: en un PDF de varias, a la izquierda salen las miniaturas (`Re Pág` / `Av Pág` para pasar).
- **Zoom**: `Ctrl` + rueda, o los botones de arriba.
- **Formato**: PNG, JPG, WEBP, TIFF, BMP, GIF o icono. En JPG y WEBP se elige la calidad.
- **Resolución y tamaño**: o pones la resolución (ppp) y salen los píxeles, o pones los píxeles y se ajusta la resolución. Con *Mantener la proporción* desmarcado se puede estirar.
- **Fondo transparente** (solo PDF): lo que en el PDF no tiene nada debajo queda transparente en vez de blanco.
- **Guardar…** (`Ctrl+S`) o **Copiar** al portapapeles (`Ctrl+C`).

La zona se vuelve a dibujar con la resolución elegida, no se amplía la vista previa. En un PDF con texto o dibujos, una zona pequeña sacada a 600 ppp sale nítida. Si lo que hay en el PDF es una foto de poca resolución, subir los ppp la hace más grande pero no le añade detalle.

## Qué convierte

| Familia | Entra | Sale | Qué usa |
|---|---|---|---|
| Imagen | png, jpg, webp, bmp, gif, tiff, ico, heic, avif, svg, psd, tga | png, jpg, webp, bmp, gif, tiff, ico, pdf | Magick.NET, va dentro del .exe |
| PDF | pdf | png, jpg, webp, bmp (una imagen por página) y tiff (todas en uno), a 200 ppp | el lector de PDF de Windows |
| Word | doc, docx, rtf, odt, html, txt, **pdf** | pdf, docx, doc, rtf, odt, html, txt, xps | Word instalado |
| Excel | xls, xlsx, xlsm, xlsb, ods | pdf, xls, xlsx, ods, html, xps | Excel instalado |
| PowerPoint | ppt, pptx, pps, ppsx, odp | pdf, pptx, ppt, odp, xps, png/jpg (una por diapositiva) | PowerPoint instalado |
| Datos | csv, tsv, json, xml, xlsx | xlsx, csv, tsv, json, xml | interno + ClosedXML |
| Texto | txt, csv, sql, xml, json, log, html, ps1… | el mismo archivo en UTF-8, UTF-8 con BOM, ANSI o UTF-16 | interno |

## Criterios fijos

- **CSV**: se escribe con `;` y en UTF-8 con BOM, para que Excel en español lo abra con las tildes bien. Al leer, el separador se adivina (`;`, `,`, tabulador o `|`).
- **De Excel a datos** (csv, json, xml): solo la primera hoja, con los valores tal como se ven en Excel.
- **A Excel desde datos**: lo que es claramente un número va como número. Lo que empieza por cero (`0021`, un código postal) se queda como texto para no perder los ceros.
- **Cambio de codificación**: el resultado se llama `nombre (UTF-8).sql`. Si al bajar a ANSI hay algún carácter que no existe en ANSI (`→`, emojis…), da error en vez de poner `?` sin avisar.
- **PDF a Word**: lo hace Word, que reconstruye el documento. Con PDF escaneados o maquetaciones complicadas puede necesitar retoques.
- **Office**: abre los archivos en solo lectura y sin macros. Si ya tenías Word, Excel o PowerPoint abiertos, no se cierran.

## Publicar

```powershell
.\publicar.ps1                                   # deja publish\Convertidor.exe
.\publicar.ps1 -Destino "$env:USERPROFILE\Desktop"
```

Es un único `.exe` autocontenido: no necesita .NET instalado. Necesita Windows 10 (1809) o posterior, por el lector de PDF. El primer arranque tarda un poco más porque extrae las librerías de imagen.

## Estructura

- `Conversion\Catalogo.cs`: decide qué conversores valen para cada archivo. El orden importa: si dos ofrecen el mismo formato, gana el primero.
- `Conversion\Conversor*.cs`: uno por familia, todos implementan `IConversor`.
- `Conversion\SesionOffice.cs`: Word, Excel y PowerPoint por COM, una instancia por tanda.
- `Conversion\Paginas\`: lo que tiene páginas que se pueden dibujar. `FuentePdf` usa `Windows.Data.Pdf` (sin Ghostscript, que es AGPL) y `FuenteImagen` usa Magick. Todo se mide en DIP (1/96 de pulgada): píxeles = DIP × ppp / 96.
- `VentanaRecorte.xaml(.cs)`: la ventana de recorte.
- `MainWindow.xaml(.cs)`: la ventana principal. Las conversiones van en un hilo STA aparte (Office lo exige) para que la ventana no se congele.

Para añadir una familia nueva hay que crear una clase `IConversor` y meterla en la lista de `Catalogo`.
