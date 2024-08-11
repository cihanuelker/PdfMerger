window.FileUtil = {
    saveAs: function (fileName, byteArray) {
        var blob = new Blob([byteArray], { type: "application/pdf" });
        var link = document.createElement('a');
        link.href = window.URL.createObjectURL(blob);
        link.download = fileName;
        link.click();
    }
};