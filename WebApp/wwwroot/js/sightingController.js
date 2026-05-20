const SightingController = (() => {
    let services;

    function init(deps) {
        services = deps;
        enableValidationAutoClear('createSightingForm');
    }

    function openModal(id, latitude, longitude) {
        document.getElementById('sightingPetId').value = id;

        bootstrap.Modal.getOrCreateInstance(
            document.getElementById('createPetSightingModal')
        ).show();

        setTimeout(() => {
            services.mapService.createMap('modalSightingMap', latitude, longitude, 13, {
                clickable: true,
                showMarker: true,
                onClick: (lat, lng) => {
                    sightingLatitude.value = lat;
                    sightingLongitude.value = lng;
                }
            });
        }, 300);
    }

    async function submitSighting() {
        const formData = new FormData(document.getElementById('createSightingForm'));
        const response = await services.petService.createSighting(formData);

        if (response.ok) {
            //alert('Sighting reported!');
            showToast('Sighting reported!');
            //location.reload();

            bootstrap.Modal
                .getInstance(document.getElementById('createPetSightingModal'))
                .hide();

            services.PetDetailsModule.init(services.petModel); // reload data only
        }
        else {
            const errors = await response.json();
            showValidationErrors(errors, 'createSightingForm');

            //alert(error.message);
        //    showToast(error.message, "error");
            showToast(errors.message ?? 'Fix validation errors', 'error');

        }
    }


    return {
        init,
        openModal,
        submitSighting
    };
})();
