// ===============================
const PetDetailsModule = (() => {

    let services;
    let map;

    function initModule(deps) {
        services = deps;
    }

    function init(pet) {
        map = services.mapService.createMap(
            "detailsMap",
            pet.Latitude,
            pet.Longitude,
            13,
            {
                clickable: false,
                showMarker: true
            }
        );

        renderSightings(pet);
    }

    function renderSightings(pet) {
        if (!pet.Sightings?.length) return;

        services.mapService.renderSightings(
            pet.Sightings,
            "detailsMap"
        );

        const points = [
            [pet.Latitude, pet.Longitude],
            ...pet.Sightings.map(s => [s.Latitude, s.Longitude])
        ];

        const bounds = L.latLngBounds(points);
        map.fitBounds(bounds, { padding: [50, 50] });
    }

    function focusSighting(lat, lng) {
        services.mapService.setView("detailsMap", lat, lng, 16);
    }

    function openSightingModal(pet) {
        services.sightingController.openModal(pet);
    }

    function enableEdit(id) {
        services.navigationHelper.goToPetDetails(id);
    }

    return {
        initModule,
        init,
        focusSighting,
        openSightingModal,
        enableEdit
    };
})();
