const PetFormController = (() => {
    let services;
    let mode = FORM_MODE.CREATE;

    function init(deps) {
        services = deps;

        const input = document.getElementById('petImageFile');

        if (input) {
            input.onchange = e => {
                const file = e.target.files[0];
                if (!file) return;

                showImagePreview('petPreview', URL.createObjectURL(file));
            };
        }

        enableValidationAutoClear('createPetForm');
    }

    function fillPetForm(pet) {
        petPetId.value = pet.petId;
        petName.value = pet.name;
        petType.value = pet.type;
        petBreed.value = pet.breed;
        petColor.value = pet.color;
        petDescription.value = pet.description;
        petLatitude.value = pet.latitude;
        petLongitude.value = pet.longitude;
        petDateLost.value = pet.dateLost?.split('T')[0];
        petStatus.value = pet.status;
        petLastSeenAddress.value = pet.lastSeenAddress;
    }

    function toggleFormFields(disabled) {
        document.querySelectorAll('#createPetForm input, #createPetForm textarea, #createPetForm select')
            .forEach(el => el.disabled = disabled);
    }

    function toggleSubmitButton(isOwner, isEdit) {
        const btn = document.getElementById('submit');

        btn.classList.toggle('d-none', !isOwner);
        btn.innerText = isEdit ? 'Update' : 'Add';

        mode = isEdit ? FORM_MODE.EDIT : FORM_MODE.CREATE;
    }


    async function submitPet() {
        const formData = new FormData(document.getElementById('createPetForm'));

        if (!formData.get('petDateLost')) {
            formData.set('petDateLost', new Date().toISOString());
        }

        const response = await services.petService.savePet(
            formData,
            mode === FORM_MODE.EDIT
        );

        if (response.ok) {
            ////alert(mode === FORM_MODE.EDIT ? 'Pet updated!' : 'Pet created!');
            //showToast(mode === FORM_MODE.EDIT ? 'Pet updated!' : 'Pet created!');
            //// location.reload();
            //setTimeout(() => location.reload(), 3000);

            showToast(mode === FORM_MODE.EDIT ? 'Pet updated!' : 'Pet created!');

            bootstrap.Modal
                .getInstance(document.getElementById('createPetModal'))
                .hide();

            services.petHomePageController.initHomePage(); // reload data only

        }
        else {
            const errors = await response.json();
            showValidationErrors(errors, 'createPetForm');
            //alert(error.message);
            showToast(errors.message??'Fix validation errors', 'error');
        }
    }

    function showImagePreview(id, url) {
        const preview = document.getElementById(id);
        preview.src = url;
        preview.style.display = 'block';
    }

    function clearImagePreview(id) {
        const preview = document.getElementById(id);
        preview.src = '';
        preview.style.display = 'none';
    }

    function updateImagePreview(url) {
        if (url) showImagePreview('petPreview', url);
        else clearImagePreview('petPreview');
    }

    return {
        init,
        fillPetForm,
        toggleFormFields,
        toggleSubmitButton,
        submitPet,
        updateImagePreview
    };
})();
