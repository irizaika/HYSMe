const LostPetPageController = (() => {

    let services;
    let searchTimeout;

    function init(deps) {
        services = deps;

        const params = new URLSearchParams(window.location.search);

        const search = params.get("search") || "";
        const page = parseInt(params.get("page") || "1");

        document.getElementById("petSearch").addEventListener("input", function () {
            clearTimeout(searchTimeout);

            searchTimeout = setTimeout(() => {
                loadPetsPage(1);
            }, 300);
        });

        document.getElementById("petSearch").value = search;
    }
    
    // same on home page
    function handleReportClick() {
        if (!services.isAuthenticated) {
            services.navigationHelper.goToLogin();
            return;
        }
        services.petModalController.openCreateModal();
    }
    


    async function loadPetsPage(page = 1) {

        const search = document.getElementById("petSearch").value;

        // Keep URL synchronized
        const url = new URL(window.location);

        url.searchParams.set("page", page);

        if (search) {
            url.searchParams.set("search", search);
        } else {
            url.searchParams.delete("search");
        }

        history.replaceState({}, "", url);

        const html = await PetService.getFiltered(search, page);

        document.getElementById("updatePetListContainer").innerHTML = html;
    }

    return {
        init,
        handleReportClick,
        loadPetsPage
    };
})();