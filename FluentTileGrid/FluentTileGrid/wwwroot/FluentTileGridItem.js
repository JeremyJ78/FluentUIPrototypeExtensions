const _instances = [];

export function initialize(id, dotNetReference) {
    var element = document.getElementById(id);
    var parent = element.parentElement;

    if (!element) {
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
        element: element,
        dotNetHelper: dotNetReference,
        parent: parent,
        orientation: 0
    };

    const resizers = element.querySelectorAll(".fluent-tile-grid-item-resize-handle");

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

    instance.originalWidth = parseFloat(getComputedStyle(instance.element, null).getPropertyValue('width').replace('px', ''));
    instance.originalHeight = parseFloat(getComputedStyle(instance.element, null).getPropertyValue('height').replace('px', ''));
    instance.originalX = instance.element.getBoundingClientRect().left;
    instance.originalY = instance.element.getBoundingClientRect().top;
    instance.originalMouseX = e.pageX;
    instance.originalMouseY = e.pageY;
    window.addEventListener('mousemove', resize);
    window.addEventListener('mouseup', stopResize);

    function resize(e) {
        if (current.classList.contains('fluent-tile-grid-cursor-nwse-resize')) {
            const width = instance.originalWidth + (e.pageX - instance.originalMouseX);
            const height = instance.originalHeight + (e.pageY - instance.originalMouseY);
            instance.orientation = 2;

            if (width > instance.minimumSize) {
                instance.newWidth = width
            }

            if (height > instance.minimumSize) {
                instance.newHeight = height
            }
        }
        else if (current.classList.contains('fluent-tile-grid-cursor-ns-resize')) {
            const height = instance.originalHeight + (e.pageY - instance.originalMouseY)
            instance.orientation = 1;

            if (height > instance.minimumSize) {
                instance.newHeight = height
            }
        }
        else if (current.classList.contains('fluent-tile-grid-cursor-ew-resize')) {
            const width = instance.originalWidth + (e.pageX - instance.originalMouseX)
            instance.orientation = 0;

            if (width > instance.minimumSize) {
                instance.newWidth = width
            }
        }
    }

    function stopResize() {
        window.removeEventListener('mousemove', resize);
        window.removeEventListener('mouseup', stopResize);
        

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

        instance.dotNetHelper.invokeMethodAsync('Resized', value);

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