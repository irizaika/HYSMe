const NavigationHelper = (() => {

    function goToPetDetails(id) {

        const params = new URLSearchParams({
            returnUrl: window.location.pathname + window.location.search
        });

        window.location.href =
            `/Pets/Details/${id}?${params.toString()}`;
    }

    function goToLogin(id) {
        window.location.href = '/Auth/Login';
    }

    return {
        goToPetDetails,
        goToLogin
    };
})();
