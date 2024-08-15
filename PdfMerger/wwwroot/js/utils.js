window.FileUtil = {
    saveAs: function (fileName, byteArray) {
        var blob = new Blob([byteArray], { type: "application/pdf" });
        var link = document.createElement('a');
        link.href = window.URL.createObjectURL(blob);
        link.download = fileName;
        link.click();
    }
};

window.PdfViewer = {
    showPdf: function (pdfData, canvasId) {
        var pdfAsArray = new Uint8Array(pdfData);
        pdfjsLib.getDocument(pdfAsArray).promise.then(function (pdf) {
            pdf.getPage(1).then(function (page) {
                var scale = 1.5;
                var viewport = page.getViewport({ scale: scale });

                var canvas = document.getElementById(canvasId);
                var context = canvas.getContext('2d');
                canvas.height = viewport.height;
                canvas.width = viewport.width;

                var renderContext = {
                    canvasContext: context,
                    viewport: viewport
                };
                page.render(renderContext);
            });
        });
    }
};
