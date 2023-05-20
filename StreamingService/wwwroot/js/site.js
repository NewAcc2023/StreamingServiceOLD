// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function NeedToLoginToSubscribeAlert() {
    alert("To Subscribe Firstly Login");
}

function NeedToLoginToLikeDislikeAlert() {
    alert("To Like Or Dislike Firstly Login");
}

function EmptyComment() {
    alert("Empty Comments Not Allowed");
}

function generateDiv(name, text, image, time) {
    let div = document.createElement('div');

    div.classList.add('comment');
    div.classList.add('mt-4');
    div.classList.add('text-justify');
    div.classList.add('float-left');
    div.classList.add('col-12');

    let img = document.createElement('img');

    img.setAttribute('src', image);
    img.classList.add('rounded-circle');
    img.width = 40;
    img.height = 40;

    let labelName = document.createElement('label');

    labelName.classList.add('fs-5');
    labelName.classList.add('fw-bold');
    labelName.classList.add('mx-2');
    labelName.textContent = name;

    let labelDate = document.createElement('label');

    labelDate.classList.add('fs-6');
    labelDate.classList.add('fw-lighter');
    labelDate.textContent = time;

    let br = document.createElement('br');

    let p = document.createElement('p');
    p.textContent = text;

    div.appendChild(img);
    div.appendChild(labelName);
    div.appendChild(labelDate);
    div.appendChild(br);
    div.appendChild(p);

    return div;
}

function showVideoUploadStatus() {
    var videoUploadStatus = document.getElementById("VideoIsUploading");
    videoUploadStatus.style.display = "block";
}
