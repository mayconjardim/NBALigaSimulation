#!/bin/bash
# Script para gerar PDF do currículo usando Chrome/Chromium headless

HTML_FILE="text.html"
PDF_FILE="curriculo.pdf"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
HTML_PATH="$SCRIPT_DIR/$HTML_FILE"
PDF_PATH="$SCRIPT_DIR/$PDF_FILE"

# Tenta encontrar Chrome/Chromium
if command -v google-chrome &> /dev/null; then
    CHROME_CMD="google-chrome"
elif command -v chromium-browser &> /dev/null; then
    CHROME_CMD="chromium-browser"
elif command -v chromium &> /dev/null; then
    CHROME_CMD="chromium"
else
    echo "Chrome/Chromium não encontrado!"
    echo "Por favor, instale o Google Chrome ou Chromium."
    exit 1
fi

echo "Gerando PDF usando $CHROME_CMD..."
echo "Arquivo HTML: $HTML_PATH"
echo "Arquivo PDF: $PDF_PATH"

# Converte caminho para formato file://
FILE_URL="file://$HTML_PATH"

# Gera o PDF usando Chrome headless
$CHROME_CMD --headless --disable-gpu --print-to-pdf="$PDF_PATH" --print-to-pdf-no-header "$FILE_URL" 2>/dev/null

if [ -f "$PDF_PATH" ]; then
    echo "PDF gerado com sucesso: $PDF_PATH"
else
    echo "Erro ao gerar PDF. Tentando método alternativo..."
    # Método alternativo usando --run-all-compositor-stages-before-draw
    $CHROME_CMD --headless --disable-gpu --run-all-compositor-stages-before-draw --print-to-pdf="$PDF_PATH" "$FILE_URL" 2>/dev/null
    
    if [ -f "$PDF_PATH" ]; then
        echo "PDF gerado com sucesso: $PDF_PATH"
    else
        echo "Erro ao gerar PDF."
        exit 1
    fi
fi
