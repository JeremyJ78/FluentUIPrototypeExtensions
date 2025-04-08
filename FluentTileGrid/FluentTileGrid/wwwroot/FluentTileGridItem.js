const _instances = [];

export function initialize(id, dotNetReference) {
    var dropZone = document.getElementById(id);
    var parent = dropZone.parentElement;
    const preview = dropZone.getElementsByClassName('fluent-tile-grid-item-preview')[0];
    const original = dropZone.getElementsByClassName('fluent-tile-grid-item-original')[0];

    if (!dropZone) {
        return;
    }

    _instances[id] = {
        minimumSize: 20,
        originalWidth: 0,
        originalHeight: 0,
        originalX: 0,
        originalY: 0,
        originalMouseX: 0,
        originalMouseY: 0,
        newWidth: 0,
        newHeight: 0,
        resizers: [],
        dotNetHelper: dotNetReference,
        parent: parent,
        orientation: 0,
        isResizable: false,
        preview: preview,
        original: original
    };

    const resizers = dropZone.querySelectorAll(".fluent-tile-grid-item-resize-handle");

    _instances[id].resizers = [];

    for (let i = 0; i < resizers.length; i++) {
        const current = resizers[i];
        current.addEventListener('mousedown', function (e) {
            beginResize(id, current, e);
        })
    }
}

function beginResize(id, current, e) {
    e.preventDefault();
    const instance = _instances[id];

    if (!instance) {
        return;
    }

    instance.originalWidth = parseFloat(getComputedStyle(instance.original, null).getPropertyValue('width').replace('px', ''));
    instance.originalHeight = parseFloat(getComputedStyle(instance.original, null).getPropertyValue('height').replace('px', ''));
    instance.originalX = instance.original.getBoundingClientRect().left;
    instance.originalY = instance.original.getBoundingClientRect().top;
    instance.originalMouseX = e.pageX;
    instance.originalMouseY = e.pageY;
    instance.isResizable = true;
    window.addEventListener('mousemove', resize);
    window.addEventListener('mouseup', stopResize);

    if (instance.preview) {
        instance.preview.style.width = instance.originalWidth;
        instance.preview.style.height = instance.originalHeight;
        instance.preview.style.display = 'block';
    }

    function resize(e) {
        const width = instance.originalWidth + (e.pageX - instance.originalMouseX);
        const height = instance.originalHeight + (e.pageY - instance.originalMouseY);


        if (current.classList.contains('fluent-tile-grid-cursor-nwse-resize')) {
            instance.orientation = 2;

            if (width > instance.minimumSize) {
                instance.newWidth = width
            }

            if (height > instance.minimumSize) {
                instance.newHeight = height
            }

            if (instance.preview) {
                instance.preview.style.width = width + "px";
                instance.preview.style.height = height + "px";
            }
        }
        else if (current.classList.contains('fluent-tile-grid-cursor-ns-resize')) {
            instance.orientation = 1;

            if (height > instance.minimumSize) {
                instance.newHeight = height
            }

            if (instance.preview) {
                instance.preview.style.height = height + "px";
                instance.preview.style.width = instance.originalWidth + "px";
            }
        }
        else if (current.classList.contains('fluent-tile-grid-cursor-ew-resize')) {
            const width = instance.originalWidth + (e.pageX - instance.originalMouseX)
            instance.orientation = 0;

            if (width > instance.minimumSize) {
                instance.newWidth = width
            }

            if (instance.preview) {
                instance.preview.style.width = width + "px";
                instance.preview.style.height = instance.originalHeight + "px";
            }
        }
    }

    async function stopResize() {
        window.removeEventListener('mousemove', resize);
        window.removeEventListener('mouseup', stopResize);
        if (instance.preview) {
            instance.preview.style.display = 'none';
            instance.isResizable = false;
        }

        const value = {
            orientation: instance.orientation,

            original: {
                width: instance.originalWidth,
                height: instance.originalHeight,
                x: instance.originalX,
                y: instance.originalY,
            },

            mousePosition: {
                x: instance.originalMouseX,
                y: instance.originalMouseY,
            },

            newSize: {
                width: instance.newWidth,
                height: instance.newHeight,
            },

            parent: {
                width: parseFloat(getComputedStyle(instance.parent, null).getPropertyValue('width').replace('px', '')),
                height: parseFloat(getComputedStyle(instance.parent, null).getPropertyValue('height').replace('px', '')),
            }
        };

        await instance.dotNetHelper.invokeMethodAsync('Resized', value);

        instance.originalWidth = parseFloat(getComputedStyle(instance.original, null).getPropertyValue('width').replace('px', ''));
        instance.originalHeight = parseFloat(getComputedStyle(instance.original, null).getPropertyValue('height').replace('px', ''));

        if (instance.preview) {
            instance.preview.style.width = instance.originalWidth;
            instance.preview.style.height = instance.originalHeight;
        }

    }
}

export function destroy(id) {
    const instance = _instances[id];

    if (instance) {
        for (let i = 0; i < instance.resizers.length; ++i) {
            instance.resizers[i].removeEventListener("mousedown", function (e) {
                beginResize(id, instance.resizers[i], e);
            });
        }
    }
}