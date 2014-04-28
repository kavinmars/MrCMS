(function() {
    'use strict';

    var common,
        mobile,
        desktop,
        rootSelector = '[data-mfnav="root"]',
        navSelector = '[data-mfnav="nav"]',
        subMenuSelector = '[data-mfnav="subMenu"]',
        menuItemSelector = '[data-mfnav="menuItem"]';

    (function() {
        $(function() {
            var $root = $(rootSelector);

            mobile.init($root);
            desktop.init($root);
            attachResponsiveHook($root);
            setMode($root);

            $(window).resize(function() {
                setMode($root);
            });
        });

        function attachResponsiveHook() {
            $('<style>@media(max-width: 767px){' + rootSelector + '{width: 767px !important;}}</style>').appendTo('HEAD');
        }

        function setMode($root) {
            var isReponsiveLayoutDesktop = $root.css('width') !== "767px";

            if (isReponsiveLayoutDesktop) {
                if ($root.data('mfnav-mode') !== 'desktop') {
                    mobile.deactivate($root);
                    desktop.activate($root);
                }

                desktop.resize($root);
                return;
            }

            if ($root.data('mfnav-mode') !== 'mobile') {
                desktop.deactivate($root);
                mobile.activate($root);
            }
        }
    }());

    common = (function() {
        function loadSubMenu($menuItem, callback) {
            var url = $menuItem.closest(navSelector).data('mfnav-url'),
                data = { parentId: $menuItem.data('mfnav-id') };

            $.get(url, data, function(response) {
                var $subMenu = appendSubMenuFromJson($menuItem, response);

                if (typeof (callback) === 'function') {
                    callback($subMenu);
                }
            });
        }

        function appendSubMenuFromJson($menuItem, data) {
            var $subMenu = $('<ul ' + subMenuSelector.replace('[', '').replace(']', '') + '>')
                .data('mfnav-level', ($menuItem.parent().data('mfnav-level') || 1) + 1)
                .hide();

            $.each(data, function() {
                $('<li ' + menuItemSelector.replace("[", "").replace("]", "") + '>')
                    .html('<a href="{url}">{text}</a>'.supplant(this))
                    .data('mfnav-id', this.id)
                    .data('mfnav-has-submenu', this.hasChildren)
                    .appendTo($subMenu);
            });

            $menuItem.append($subMenu);

            return $subMenu;
        }

        return {
            loadSubMenu: loadSubMenu
        };
    }());

    desktop = (function() {
        function init($root) {
            var $nav = $root.children(navSelector);

            $nav
                .on('mouseenter.mfnav', menuItemSelector, onMouseEnter)
                .on('mouseleave.mfnav', menuItemSelector, onMouseLeave);

            $nav
                .find(subMenuSelector)
                .addClass('dropdown-menu');
        }

        function activate($root) {
            $root.data('mfnav-mode', 'desktop');
            resize($root);
        }

        function deactivate($root) {
            $root.data('mfnav-mode', '');
        }

        function resize($root) {
            var $nav = $root.children(navSelector);

            $nav.children(menuItemSelector).has(subMenuSelector).each(function() {
                var $menuItem = $(this),
                    $subMenu = $menuItem.children(subMenuSelector),
                    rightOffset = $(window).width() - ($menuItem.offset().left + $menuItem.outerWidth()),
                    align = rightOffset < ($subMenu.outerWidth() * 3) ? 'left' : 'right';

                $subMenu
                    .removeClass('submenu-align-left')
                    .removeClass('submenu-align-right')
                    .addClass('submenu-align-' + align);
            });
        }

        function onMouseEnter(event) {
            var $menuItem = $(event.currentTarget),
                $subMenu = $menuItem.children(subMenuSelector);

            if ($subMenu.length) {
                showSubMenu($subMenu);
                return;
            }

            tryLoadSubMenu($menuItem);
        }

        function onMouseLeave(event) {
            var $menuItem = $(event.currentTarget),
                $subMenu = $menuItem.children(subMenuSelector);

            if ($subMenu.length) {
                hideSubMenu($subMenu);
            }
        }

        function showSubMenu($subMenu) {
            $subMenu.show();
        }

        function hideSubMenu($subMenu) {
            $subMenu.hide();
        }

        function tryLoadSubMenu($menuItem) {
            var level = ($menuItem.parent().data('mfnav-level') || 1),
                maxLevel = $menuItem.closest(navSelector).data('mfnav-max-levels'),
                hasSubMenu = $menuItem.data('mfnav-has-submenu');

            if (level < maxLevel && hasSubMenu) {
                common.loadSubMenu($menuItem, onSubMenuLoaded);
                return true;
            }

            return false;
        }

        function onSubMenuLoaded($subMenu) {
            $subMenu.addClass('dropdown-menu');
            showSubMenu($subMenu);
        }

        return {
            init: init,
            activate: activate,
            deactivate: deactivate,
            resize: resize
        };
    }());

    mobile = (function() {
        var sidrSelector = '#mfnav-mobile',
            headerSelector = '[data-mfnav="mobileHeader"]',
            crumbsSelector = '[data-mfnav="mobileCrumbs"]';

        function init() {
            $().sidr({
                name: sidrSelector.replace('#', ''),
                source: '[data-mfnav="mobile"], ' + rootSelector
            });

            var $sidr = $(sidrSelector);

            $sidr
                .on('click.mfnav', menuItemSelector + ' > A', onClickLink)
                .on('click.mfnav', '[data-mfnav="mobileBack"]', onClickBack);

            $sidr
                .find(headerSelector)
                .hide();

            $sidr
                .find(navSelector)
                .addClass('sidr-class-menu');

            $sidr
                .find(subMenuSelector)
                .addClass('sidr-class-submenu')
                .hide();

            $(document)
                .on('click.mfnav', '[data-mfnav="toggleMobileNav"]', onClicktoggleMobileNav);
        }

        function activate($root) {
            $root.data('mfnav-mode', 'mobile');
        }

        function deactivate($root) {
            $root.data('mfnav-mode', '');
            $.sidr('close', sidrSelector.replace('#', ''));
        }

        function onClicktoggleMobileNav(event) {
            event.preventDefault();
            $.sidr('toggle', sidrSelector.replace('#', ''));
        }

        function onClickLink(event) {
            var $menuItem = $(event.currentTarget).closest(menuItemSelector),
                $subMenu = $menuItem.children(subMenuSelector);

            if ($subMenu.length) {
                event.preventDefault();
                showSubMenu($subMenu);
                return;
            }

            if (tryLoadSubMenu($menuItem)) {
                event.preventDefault();
            }
        }

        function onClickBack(event) {
            event.preventDefault();

            var $sidr = $(event.delegateTarget),
                $crumbs = $sidr.find(crumbsSelector),
                $lastCrumb = $crumbs.children().last(),
                $subMenu = $lastCrumb.data('mfnav-submenu'),
                $parentMenu = $subMenu.parents(subMenuSelector);

            $lastCrumb.remove();
            updateHeader($sidr, $parentMenu);

            $subMenu.animate({ left: 260 }).promise().done(function() {
                $subMenu.hide();

                $sidr
                    .find(navSelector)
                    .height($parentMenu.length ? $parentMenu.height() : '100%');
            });
        }

        function showSubMenu($subMenu) {
            var $nav = $subMenu.closest(navSelector),
                $sidr = $nav.closest(sidrSelector);

            $nav.height($subMenu.height());
            $subMenu.show().animate({ left: 0 });

            $sidr
                .find(crumbsSelector)
                .append($('<div>')
                    .data('mfnav-submenu', $subMenu)
                    .html($subMenu.siblings('A').html()));

            updateHeader($sidr, $subMenu);
        }

        function updateHeader($sidr, $subMenu) {
            var $header = $sidr.find(headerSelector);

            if (!$subMenu.length) {
                $header.hide();
                return;
            }

            $header
                .show()
                .find('SPAN')
                .html($subMenu.siblings('A').html());
        }

        function tryLoadSubMenu($menuItem) {
            var level = ($menuItem.parent().data('mfnav-level') || 1),
                maxLevel = $menuItem.closest(navSelector).data('mfnav-max-levels'),
                hasSubMenu = $menuItem.data('mfnav-has-submenu');

            if (level < maxLevel && hasSubMenu) {
                common.loadSubMenu($menuItem, onSubMenuLoaded);
                return true;
            }

            return false;
        }

        function onSubMenuLoaded($subMenu) {
            $subMenu.addClass('sidr-class-submenu');

            $subMenu.children(menuItemSelector).each(function() {
                var $menuItem = $(this);

                if ($menuItem.data('mfnav-has-submenu')) {
                    $menuItem.addClass('sidr-class-has-submenu');
                }
            });

            showSubMenu($subMenu);
        }

        return {
            init: init,
            activate: activate,
            deactivate: deactivate
        };
    }());
}());