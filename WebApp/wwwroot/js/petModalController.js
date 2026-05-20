const PetModalController = (() => {
    let services;
    let mode = FORM_MODE.CREATE;

    function init(deps) {
        services = deps;

        document.getElementById('createPetModal')
            .addEventListener('shown.bs.modal', handleModalShown);
    }

    function handleModalShown() {
        const saved = JSON.parse(localStorage.getItem('userLocation'));

        if (mode === FORM_MODE.CREATE) {
            if (saved) {
                initModalMap(saved.lat, saved.lng, saved.zoom);
            } else {
                initModalMap(56.9496, 24.1052, 13);
            }
        }
    }

    function initModalMap(lat, lng, zoom) {
        services.mapService.createMap('modalPetMap', lat, lng, zoom, {
            clickable: mode !== FORM_MODE.VIEW,
            showMarker: true,
            onClick: (lat, lng) => {
                petLatitude.value = lat;
                petLongitude.value = lng;

                //it will clear error field if any error exists
                document.getElementById('petLatitude').dispatchEvent(new Event('input')); 
                document.getElementById('petLongitude').dispatchEvent(new Event('input'));
            }
        });
    }

    function openCreateModal() {
        mode = FORM_MODE.CREATE;

        const form = document.getElementById('createPetForm');
        form.reset();

        document.getElementById('petPetId').value = '';

        services.petFormController.toggleFormFields(false);
        services.petFormController.toggleSubmitButton(true, false);
        services.petFormController.updateImagePreview(null);

        services.mapService.renderSightings([], 'modalPetMap');

        bootstrap.Modal.getOrCreateInstance(
            document.getElementById('createPetModal')
        ).show();
    }

    async function openEditModal(petId, isOwner) {
        const data = await services.petService.getById(petId);
        const pet = data.result ?? data;

        mode = isOwner ? FORM_MODE.EDIT : FORM_MODE.VIEW;

        services.petFormController.toggleFormFields(!isOwner);
        services.petFormController.toggleSubmitButton(isOwner, isOwner);
        services.petFormController.fillPetForm(pet);

        bootstrap.Modal.getOrCreateInstance(
            document.getElementById('createPetModal')
        ).show();

        setTimeout(() => {
            services.petFormController.updateImagePreview(pet.imageUrl);
            initModalMap(pet.latitude, pet.longitude, 10);
        }, 300);
    }

    return {
        init,
        openCreateModal,
        openEditModal
    };
})();