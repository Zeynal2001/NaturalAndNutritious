let formFile = document.getElementById('formFile');

formFile.addEventListener('change', () => {
    let val = formFile.files;
    document.getElementById('profile-image-show').src = `${URL.createObjectURL(val[0])}`
});