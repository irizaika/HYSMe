const PetPageController = (() => {
    let services;

    function init(deps) {
        services = deps;
    }

    function saveLocation(lat, lng, zoom) {
        localStorage.setItem('userLocation', JSON.stringify({
            lat,
            lng,
            zoom,
            timestamp: Date.now()
        }));
    }

    function getSavedLocation() {
        const raw = localStorage.getItem('userLocation');
        if (!raw) return null;

        const parsed = JSON.parse(raw);
        const age = Date.now() - parsed.timestamp;

        if (age > 1000 * 60 * 60) return null;

        return parsed;
    }
    function getUserLocation() {
        if (!navigator.geolocation) {
            initMainMap();
            return;
        }

        navigator.geolocation.getCurrentPosition(
            pos => {
                const lat = pos.coords.latitude;
                const lng = pos.coords.longitude;
                const zoom = 13;

                saveLocation(lat, lng, zoom);
                initMainMap(lat, lng, zoom);
            },
            () => initMainMap(),
            {
                enableHighAccuracy: false,
                maximumAge: 60000
            }
        );
    }


    function initMainMap(lat = 56.9496, lng = 24.1052, zoom = 13) {
        const existing = services.mapService.getMap('mainMap');

        if (!existing) {
            services.mapService.createMap('mainMap', lat, lng, zoom, {
                clickable: false,
                showMarker: false
            });

            loadPetsInView();

            services.mapService.onMoveEnd('mainMap', map => {
                loadPetsInView();

                const center = map.getCenter();
                saveLocation(center.lat, center.lng, map.getZoom());
            });
        }
        else {
            services.mapService.setView('mainMap', lat, lng, zoom);
            saveLocation(lat, lng, zoom);
            loadPetsInView();
        }
    }

    async function loadPetsInView() {
        const bounds = services.mapService.getBounds('mainMap');
        const pets = await services.petService.getByBounds(bounds);

        if (!pets) return;

        renderPets(pets);
    }

    function renderPets(pets) {
        services.mapService.renderMarkers(
            'mainMap',
            pets,
            services.petRenderer.createPopupHtml
        );

        services.petRenderer.renderList(pets, pet => {
            services.navigationHelper.goToPetDetails(pet.petId);
        });
    }

    function initHomePage() {
        const saved = getSavedLocation();

        if (saved) {
            initMainMap(saved.lat, saved.lng, saved.zoom);
        }
        else {
            initMainMap();
            getUserLocation();
        }
    }

    function handleReportClick() {
        if (!services.isAuthenticated) {
            services.navigationHelper.goToLogin();
            return;
        }

        services.petModalController.openCreateModal();
    }

    return {
        init,
        initHomePage,
        handleReportClick,
        loadPetsInView
    };
})();