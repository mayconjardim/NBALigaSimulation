#!/usr/bin/env python3
"""
Script para gerar PDF a partir do arquivo HTML do currículo
Usa playwright para renderizar o HTML e gerar PDF idêntico
"""

import os
import sys
import subprocess

def install_playwright():
    """Instala playwright se necessário"""
    try:
        import playwright
    except ImportError:
        print("Instalando playwright...")
        subprocess.check_call([sys.executable, "-m", "pip", "install", "playwright", "--break-system-packages"])
        print("Instalando navegadores do playwright...")
        subprocess.check_call([sys.executable, "-m", "playwright", "install", "chromium"])

def generate_pdf():
    """Gera PDF usando playwright"""
    # Verifica se playwright está instalado
    try:
        from playwright.sync_api import sync_playwright
    except ImportError:
        print("Playwright não está instalado. Instalando...")
        install_playwright()
        # Tenta importar novamente
        try:
            from playwright.sync_api import sync_playwright
        except ImportError:
            print("Erro: Não foi possível instalar playwright.")
            return False
    
    try:
        script_dir = os.path.dirname(os.path.abspath(__file__))
        html_file = os.path.join(script_dir, 'text.html')
        pdf_file = os.path.join(script_dir, 'curriculo.pdf')
        
        # Converte para URL file://
        html_path = os.path.abspath(html_file)
        file_url = f"file://{html_path}"
        
        print(f"Gerando PDF de {html_file}...")
        print(f"Salvando em: {pdf_file}")
        
        with sync_playwright() as p:
            browser = p.chromium.launch()
            page = browser.new_page()
            page.goto(file_url, wait_until='networkidle')
            
            # Gera o PDF com configurações para manter o layout
            page.pdf(
                path=pdf_file,
                format='A4',
                print_background=True,
                margin={
                    'top': '0.3cm',
                    'right': '0.8cm',
                    'bottom': '0.3cm',
                    'left': '0.8cm'
                }
            )
            
            browser.close()
        
        print(f"✓ PDF gerado com sucesso: {pdf_file}")
        return True
        
    except Exception as e:
        print(f"Erro ao gerar PDF: {e}")
        import traceback
        traceback.print_exc()
        return False

def main():
    print("=== Gerador de PDF do Currículo ===\n")
    
    if generate_pdf():
        print("\n✓ Concluído!")
    else:
        print("\n✗ Falha ao gerar PDF.")
        sys.exit(1)

if __name__ == "__main__":
    main()
