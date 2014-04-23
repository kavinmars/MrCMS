(function() {
    var mobile,
        desktop,
        currentMode,
        settings =
        {
            url: '/MobileFriendlyNavigation/GetChildNodes',
            maxLevels: 3
        };

    function init() {
        $(window).resize(switchMode);
        switchMode();
    }

    function switchMode() {
        var $root = $('[data-mfnav="root"]'),
            isMobileVisible = $root.find('[data-mfnav="mobile"]').css('display') === 'block';

        if (isMobileVisible && currentMode !== 'mobile') {
            mobile.activate($root);
        } else if (!isMobileVisible && currentMode !== 'desktop') {
            desktop.activate($root);
        }
    }

    function showSubMenu($menuItem) {
        if ($menuItem.data('mfnav-loaded')) {
            $menuItem.children('[data-mfnav="menu"]').show();
        } else if ($menuItem.data('mfnav-level') < settings.maxLevels) {
            loadNavigation($menuItem);
        }
    }

    function hideSubMenu($menuItem) {
        $menuItem.find('[data-mfnav="menu"]').hide();
    }

    function loadNavigation($menuItem) {
        var data =
        {
            parentId: $menuItem.data('mfnav-id')
        };

        $.get(settings.url, data, function(response) {
            appendTemplates($menuItem, response);
        });

        $menuItem.data('mfnav-loaded', true);
    }

    function appendTemplates($menuItem, data) {
        var $subNav = $('[data-mfnav-template="container"]').clone(),
            template = $('<div>').append($subNav.find('[data-mfnav-template="node"]').clone().removeAttr('data-mfnav-template')).html(),
            level = ($menuItem.data('mfnav-level') || 1) + 1;

        $subNav.find('[data-mfnav-template="node"]').remove();
        $subNav.removeAttr('data-mfnav-template');

        $.each(data, function() {
            this.level = level;
            this.class = ''; //todo: assign active class if appropriate
            $subNav.append($(template.supplant(this)));
        });

        $menuItem.append($subNav);
    }

    desktop = (function() {
        function activate($root) {
            currentMode = 'desktop';

            $root
                .find('[data-mfnav="menu"]')
                .hide();

            $root
                .off('mouseenter.mfnav')
                .off('mouseleave.mfnav')
                .off('click.mfnav')
                .on('mouseenter.mfnav', '[data-mfnav="menuItem"]', onMouseEnter)
                .on('mouseleave.mfnav', '[data-mfnav="menuItem"]', onMouseLeave);
        }

        function onMouseEnter(event) {
            var $menuItem = $(event.currentTarget);

            if ($menuItem.data('mfnav-has-children')) {
                showSubMenu($menuItem);
            }
        }

        function onMouseLeave(event) {
            var $menuItem = $(event.currentTarget);

            if ($menuItem.data('mfnav-has-children')) {
                hideSubMenu($menuItem);
            }
        }

        return {
            activate: activate
        };
    }());

    mobile = (function() {
        function activate($root) {
            currentMode = 'mobile';

            $root
                .find('[data-mfnav="mobile"] [data-mfnav="mobileCrumbs"]')
                .empty();

            $root
                .off('mouseenter.mfnav')
                .off('mouseleave.mfnav')
                .off('click.mfnav')
                .on('click.mfnav', '[data-mfnav="menuItem"] A', onClickLink)
                .on('click.mfnav', '[data-mfnav="mobileBack"]', onClickMobileBack);

            setHeader($root.find('[data-mfnav="mobile"]'));
        }

        function onClickLink(event) {
            var $mobile = $(event.delegateTarget).find('[data-mfnav="mobile"]'),
                $menuItem = $(event.currentTarget).closest('[data-mfnav="menuItem"]'),
                hasChildren = $menuItem.data('mfnav-has-children'),
                level = ($menuItem.data('mfnav-level') || 1);

            if (hasChildren && level < settings.maxLevels) {
                event.preventDefault();
                event.stopPropagation();
                addToMobileMenu($mobile, $menuItem);
                return;
            }
        }

        function onClickMobileBack(event) {
            var $mobile = $(event.delegateTarget).find('[data-mfnav="mobile"]');

            event.preventDefault();
            removeLastFromMobileMenu($mobile);
        }

        function addToMobileMenu($mobile, $menuItem) {
            var $breadcrumbs = $mobile.find('[data-mfnav="mobileCrumbs"]'),
                $link = $menuItem.find('A');

            console.log($menuItem);
            showSubMenu($menuItem);
            setHeader($mobile, $link);
            $breadcrumbs.append($('<div>').data('mfnav-menu-item', $menuItem).html($link.html()));
        }

        function removeLastFromMobileMenu($mobile) {
            var $breadcrumbs = $mobile.find('[data-mfnav="mobileCrumbs"]'),
                $lastCrumb = $breadcrumbs.children().last(),
                $link = null;

            hideSubMenu($lastCrumb.data('mfnav-menu-item'));
            $lastCrumb.remove();

            if ($breadcrumbs.children().length) {
                $link = $breadcrumbs.children().last().data('mfnav-menu-item').find('A');
            }

            setHeader($mobile, $link);
        }

        function setHeader($mobile, $link) {
            var $header = $mobile.find('[data-mfnav="mobileHeader"]'),
                text;

            if (!$link) {
                $header.hide().find('SPAN').html('');
                return;
            }

            text = $link.html(); // todo: concat?
            $header.show().find('SPAN').html(text);
        }

        return {
            activate: activate
        };
    }());

    $(init);
}());