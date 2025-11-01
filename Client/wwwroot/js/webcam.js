export const startVideo = async (dotnetRef) => {
    if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
        const stream = await navigator.mediaDevices.getUserMedia({
            video: {
                facingMode: { ideal: "user" },
                width: { min: 480, ideal: 1920, max: 1920 },
            }
        });

        await dotnetRef.invokeMethodAsync('DisplayWebcam');

        const video = document.getElementById('videoFeed');

        if ("srcObject" in video) {
            video.srcObject = stream;
        } else {
            video.src = window.URL.createObjectURL(stream);
        }

        video.onloadedmetadata = (_) => {
            video.play();
        };
    }
}

export const getFrame = async (quality) => {
    const video = document.getElementById('videoFeed');
    const w = video.clientWidth;
    const h = video.clientHeight;

    const canvas = new OffscreenCanvas(w, h);
    const ctx = canvas.getContext('2d');
    ctx.drawImage(video, 0, 0, w, h);

    return await canvas.convertToBlob({ type: 'image/jpeg', quality: quality });
};

export const drawOnFrame = (x, y, width, height) => {
    const canvas = getSizedCanvas();
    const ctx = canvas.getContext("2d");
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    ctx.beginPath();
    ctx.lineWidth = "3";
    ctx.strokeStyle = "red";
    ctx.rect(x, y, width, height);
    ctx.stroke();
}

export const clearFrame = () => {
    const canvas = getSizedCanvas();
    const ctx = canvas.getContext("2d");
    ctx.clearRect(0, 0, canvas.width, canvas.height);
}

const getSizedCanvas = () => {
    const canvas = document.getElementById('faceHighlight');
    canvas.width = canvas.clientWidth;
    canvas.height = canvas.clientHeight;
    return canvas;
}