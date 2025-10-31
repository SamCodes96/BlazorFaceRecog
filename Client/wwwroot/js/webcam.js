async function startVideo(src, dotnetRef) {
    if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
        const stream = await navigator.mediaDevices.getUserMedia({
            video: {
                facingMode: { ideal: "user" },
                width: { min: 480, ideal: 1920, max: 1920 },
            }
        });

        await dotnetRef.invokeMethodAsync('DisplayWebcam');

        const video = document.getElementById(src);

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

function getFrame(src, dest) {
    const video = document.getElementById(src);
    const canvas = document.getElementById(dest);
    canvas.getContext('2d').drawImage(video, 0, 0, canvas.width, canvas.height);

    return {
        dataUrl: canvas.toDataURL("image/jpeg")
    }
}

function drawOnFrame(src, x, y, width, height) {
    const ctx = clearFrame(src);

    ctx.beginPath();
    ctx.lineWidth = "3";
    ctx.strokeStyle = "red";
    ctx.rect(x, y, width, height);
    ctx.stroke();
}

function clearFrame(src) {
    const canvas = document.getElementById(src);
    const ctx = canvas.getContext("2d");
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    return ctx;
}