(function() {
    'use strict';

    var common,
        mobile,
        desktop;

    (function() {
        function init() {
            var $root = $('[data-mfnav="root"]');

            attachResponsiveHook($root);
            setMode($root);
            attachRootChildren($root);

            $(window).resize(function() {
                setMode($root);
            });
        }

        function attachResponsiveHook($root) {
            $('<div data-mfnav="responsiveHook" style="display:none;width:1000px;">').appendTo($root);
            $('<style>@media(max-width: 767px){[data-mfnav="responsiveHook"]{width: 767px !important;}}</style>').appendTo('HEAD');
        }

        function setMode($root) {
            var isReponsiveLayoutMobile = $root.find('[data-mfnav="responsiveHook"]').css('width') === "767px";

            if (isReponsiveLayoutMobile && $root.data('mfnav-mode') !== 'mobile') {
                mobile.activate($root);
            } else if (!isReponsiveLayoutMobile && $root.data('mfnav-mode') !== 'desktop') {
                desktop.activate($root);
            }
        }

        function attachRootChildren($root) {
            $root.find('[data-mfnav="menuItem"]').each(function() {
                var $rootMenuItem = $(this),
                    children = $rootMenuItem.data('mfnav-children');

                $rootMenuItem.removeAttr('data-mfnav-children');

                if (children && children.length) {
                    common.appendTemplates($rootMenuItem, children);
                    $rootMenuItem.data('mfnav-has-children', true);
                    calculateAlignment($rootMenuItem);
                }
            });
        }

        function calculateAlignment($rootMenuItem) {
            var offset = $rootMenuItem.offset().left,
                align = offset > 300 ? 'left' : 'right';

            $rootMenuItem
                .attr('data-mfnav-align', align);
        }

        $(init);
    }());

    common = (function() {
        function showSubMenu($menuItem) {
            var $root = $menuItem.closest('[data-mfnav="root"]');

            if ($menuItem.data('mfnav-children-loaded')) {
                $menuItem.children('[data-mfnav="menu"]').show();
            } else if ($menuItem.data('mfnav-level') < $root.data('mfnav-max-levels')) {
                loadNavigation($root, $menuItem);
            }
        }

        function hideSubMenu($menuItem) {
            $menuItem.find('[data-mfnav="menu"]').hide();
        }

        function loadNavigation($root, $menuItem) {
            var url = $root.data('mfnav-url'),
                data = { parentId: $menuItem.data('mfnav-id') };

            $.get(url, data, function(response) {
                appendTemplates($menuItem, response);
                showSubMenu($menuItem);
                setAlignment($menuItem);
            });
        }

        function appendTemplates($menuItem, data) {
            var $subMenu = $('[data-mfnav-template="menu"]').clone().removeAttr('data-mfnav-template').attr('data-mfnav', 'menu'),
                $subMenuItem,
                subMenuItemTemplate = $('<div>').append($subMenu.find('[data-mfnav-template="menuItem"]').clone().removeAttr('data-mfnav-template').attr('data-mfnav', 'menuItem')).html(),
                subMenuItemLevel = ($menuItem.data('mfnav-level') || 0) + 1;

            $subMenu
                .find('[data-mfnav-template="menuItem"]')
                .remove();

            $.each(data, function() {
                $subMenuItem = $(subMenuItemTemplate.supplant(this))
                    .data('mfnav-id', this.id)
                    .data('mfnav-level', subMenuItemLevel)
                    .data('mfnav-has-children', this.hasChildren);

                $subMenu.append($subMenuItem);
            });

            $menuItem
                .data('mfnav-children-loaded', true)
                .append($subMenu);
        }

        function setAlignment($menuItem) {
            var align = $menuItem.closest('[data-mfnav-align]').data('mfnav-align');

            $menuItem
                .children('[data-mfnav="menu"]')
                .addClass('dropdown-menu-' + align);
        }

        return {
            showSubMenu: showSubMenu,
            hideSubMenu: hideSubMenu,
            appendTemplates: appendTemplates
        };
    }());

    desktop = (function() {
        function activate($root) {
            $root
                .find('[data-mfnav="mobile"]')
                .hide();

            $root
                .data('mfnav-mode', 'desktop')
                .off('mouseenter.mfnav')
                .off('mouseleave.mfnav')
                .off('click.mfnav')
                .on('mouseenter.mfnav', '[data-mfnav="menuItem"]', onMouseEnter)
                .on('mouseleave.mfnav', '[data-mfnav="menuItem"]', onMouseLeave);
        }

        function onMouseEnter(event) {
            var $menuItem = $(event.currentTarget);

            if ($menuItem.data('mfnav-has-children')) {
                common.showSubMenu($menuItem);
            }
        }

        function onMouseLeave(event) {
            var $menuItem = $(event.currentTarget);

            if ($menuItem.data('mfnav-has-children')) {
                common.hideSubMenu($menuItem);
            }
        }

        return {
            activate: activate
        };
    }());

    mobile = (function() {
        function activate($root) {
            $root
                .find('[data-mfnav="mobile"]')
                .show()
                .find('[data-mfnav="mobileCrumbs"]')
                .empty();

            $root
                .data('mfnav-mode', 'mobile')
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

            common.showSubMenu($menuItem);
            setHeader($mobile, $link);
            $breadcrumbs.append($('<div>').data('mfnav-menu-item', $menuItem).html($link.html()));
        }

        function removeLastFromMobileMenu($mobile) {
            var $breadcrumbs = $mobile.find('[data-mfnav="mobileCrumbs"]'),
                $lastCrumb = $breadcrumbs.children().last(),
                $link = null;

            common.hideSubMenu($lastCrumb.data('mfnav-menu-item'));
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
}());