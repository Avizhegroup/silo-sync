function printFullCharts(elementSelector) {
    var printWindow = window.open('', '', 'height=600,width=800');
    var printableElements = document.querySelectorAll(elementSelector);

    printWindow.document.write(`
        <html>
        <head>
            <title>Print Charts</title>
            <style>
                @media print {
                    @page {
                        size: A3 landscape;
                        margin: 0; 
                    }
                    body {
                        margin: 0;
                        padding: 1cm;
                    }
                    .printable-element {
                        page-break-inside: avoid; 
                    }
                }
            </style>
        </head>
        <body>
    `);

    printableElements.forEach(element => {
        printWindow.document.write('<div class="printable-element">' + element.outerHTML + '</div>');
    });

    printWindow.document.write(`
        </body>
        </html>
    `);

    printWindow.document.close();
    printWindow.focus(); 
    printWindow.print();
    printWindow.close();
}

