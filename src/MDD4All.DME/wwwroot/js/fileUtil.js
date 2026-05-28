// window.saveAsFile: For downloading files (e.g. JSON exports)
window.saveAsFile = function (filename, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};

// window.initResizer: For the draggable explorer sidebar
window.initResizer = () => {
    const onMouseMove = (e) => {
        const newWidth = e.clientX;
        // Limits: Explorer between 150px and 800px
        if (newWidth > 150 && newWidth < 800) {
            document.documentElement.style.setProperty('--explorer-width', `${newWidth}px`);
        }
    };

    const onMouseUp = () => {
        document.removeEventListener('mousemove', onMouseMove);
        document.removeEventListener('mouseup', onMouseUp);
    };

    document.addEventListener('mousemove', onMouseMove);
    document.addEventListener('mouseup', onMouseUp);
};