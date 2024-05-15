let profile_inp = document.getElementById('profile-image-inp');

profile_inp.addEventListener('change', () => {
    let val = profile_inp.files;
    document.getElementById('profile-image-cont').style = `background-image: url('${URL.createObjectURL(val[0])}')`;
});