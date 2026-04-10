const SightingController = (() => {
    let services;

    function init(deps) {
        services = deps;
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
            alert('Sighting reported!');
            location.reload();
        }
        else {
            const error = await response.json();
            alert(error.message);
        }
    }

    return {
        init,
        openModal,
        submitSighting
    };
})();
