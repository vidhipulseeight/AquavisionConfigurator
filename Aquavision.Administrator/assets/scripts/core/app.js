// Helper function to calculate sidebar height for fixed sidebar layout.
var calculateFixedSidebarViewportHeight = function () {
	var sidebarHeight = $(window).height() - $('.header').height() + 1;
	if ($('body').hasClass("page-footer-fixed")) {
		sidebarHeight = sidebarHeight - $('.footer').outerHeight();
	}

	return sidebarHeight;
};

var getViewPort = function () {
	var e = window, a = 'inner';
	if (!('innerWidth' in window)) {
		a = 'client';
		e = document.documentElement || document.body;
	}
	return {
		width: e[a + 'Width'],
		height: e[a + 'Height']
	};
};

var App = function () {
    var sidebarWidth = 225;
    var sidebarCollapsedWidth = 35;

    var responsiveHandlers = [];

    // initializes main settings
    var handleInit = function () {
        var deviceAgent = navigator.userAgent.toLowerCase();
        if (deviceAgent.match(/(iphone|ipod|ipad)/)) {
            $(document).on('focus', 'input, textarea', function () {
                $('.page-header').hide();
                $('.page-footer').hide();
            });
            $(document).on('blur', 'input, textarea', function () {
                $('.page-header').show();
                $('.page-footer').show();
            });
        }
    };

	var handleSidebarState = function() {
		// remove sidebar toggler if window width smaller than 992(for tablet and phone mode)
		var viewport = getViewPort();
		if (viewport.width < 992) {
			$('body').removeClass('page-sidebar-closed');
		}
	};

    // runs callback functions set by App.addResponsiveHandler().
	var runResponsiveHandlers = function() {
		// reinitialize other subscribed elements
		for (let i = 0; i < responsiveHandlers.length; i++) {
			const each = responsiveHandlers[i];
			each.call();
		}
	};

    // Set proper height for sidebar and content. The content and sidebar height must be synced always.
	var handleSidebarAndContentHeight = function() {
		const content = $('.page-content');
		const sidebar = $('.page-sidebar');
		const body = $('body');
		var height;

		if (body.hasClass('page-footer-fixed') === true && body.hasClass('page-sidebar-fixed') === false) {
			const availableHeight = $(window).height() - $('.footer').outerHeight() - $('.header').outerHeight();
			if (content.height() < availableHeight) {
				content.attr('style', 'min-height:' + availableHeight + 'px !important');
			}
		} else {
			if (body.hasClass('page-sidebar-fixed')) {
				height = calculateFixedSidebarViewportHeight();
			} else {
				height = sidebar.height() + 20;
				const headerHeight = $('.header').outerHeight();
				const footerHeight = $('.footer').outerHeight();
				if ($(window).width() > 1024 && (height + headerHeight + footerHeight) < $(window).height()) {
					height = $(window).height() - headerHeight - footerHeight;
				}
			}
			if (height >= content.height()) {
				content.attr('style', 'min-height:' + height + 'px !important');
			}
		}
	};

    // Handle sidebar menu
	var handleSidebarMenu = function() {
		jQuery('.page-sidebar').on('click',
			'li > a',
			function(e) {
				if ($(this).next().hasClass('sub-menu') === false) {
					if ($('.btn-navbar').hasClass('collapsed') === false) {
						$('.btn-navbar').click();
					}
					return;
				}

				if ($(this).next().hasClass('sub-menu always-open')) {
					return;
				}

				var parent = $(this).parent().parent();
				var the = $(this);
				var menu = $('.page-sidebar-menu');
				var sub = jQuery(this).next();

				var autoScroll = menu.data('auto-scroll') ? menu.data('auto-scroll') : true;
				var slideSpeed = menu.data('slide-speed') ? parseInt(menu.data('slide-speed')) : 200;

				parent.children('li.open').children('a').children('.arrow').removeClass('open');
				parent.children('li.open').children('.sub-menu:not(.always-open)').slideUp(200);
				parent.children('li.open').removeClass('open');

				var slideOffeset = -200;

				if (sub.is(':visible')) {
					jQuery('.arrow', jQuery(this)).removeClass('open');
					jQuery(this).parent().removeClass('open');
					sub.slideUp(slideSpeed,
						function() {
							if (autoScroll === true && $('body').hasClass('page-sidebar-closed') === false) {
								if ($('body').hasClass('page-sidebar-fixed')) {
									menu.slimScroll({ 'scrollTo': the.position().top });
								} else {
									App.scrollTo(the, slideOffeset);
								}
							}
							handleSidebarAndContentHeight();
						});
				} else {
					jQuery('.arrow', jQuery(this)).addClass('open');
					jQuery(this).parent().addClass('open');
					sub.slideDown(slideSpeed,
						function() {
							if (autoScroll === true && $('body').hasClass('page-sidebar-closed') === false) {
								if ($('body').hasClass('page-sidebar-fixed')) {
									menu.slimScroll({ 'scrollTo': the.position().top });
								} else {
									App.scrollTo(the, slideOffeset);
								}
							}
							handleSidebarAndContentHeight();
						});
				}

				e.preventDefault();
			});
	};

    // Handles fixed sidebar
	var handleFixedSidebar = function() {
		const menu = $('.page-sidebar-menu');

		if (menu.parent('.slimScrollDiv').length === 1) { // destroy existing instance before updating the height
			menu.slimScroll({
				destroy: true
			});
			menu.removeAttr('style');
			$('.page-sidebar').removeAttr('style');
		}

		if ($('.page-sidebar-fixed').length === 0) {
			handleSidebarAndContentHeight();
			return;
		}

		const viewport = getViewPort();
		if (viewport.width >= 992) {
			const sidebarHeight = calculateFixedSidebarViewportHeight();

			menu.slimScroll({
				size: '7px',
				color: '#a1b2bd',
				opacity: .3,
				position: 'right',
				height: sidebarHeight,
				allowPageScroll: false,
				disableFadeOut: false
			});
			handleSidebarAndContentHeight();
		}
	};

	// handle the layout reinitialization on window resize
	var handleResponsiveOnResize = function () {
		var resize;
		$(window).resize(function () {
			if (resize) {
				clearTimeout(resize);
			}
			resize = setTimeout(function () {
					handleSidebarState();
					handleSidebarAndContentHeight();
					handleFixedSidebar();
					runResponsiveHandlers();
				},
				50); // wait 50ms until window resize finishes.
		});
	};

    // Handles the sidebar menu hover effect for fixed sidebar.
    var handleFixedSidebarHoverable = function () {
        if ($('body').hasClass('page-sidebar-fixed') === false) {
            return;
        }

        $('.page-sidebar').off('mouseenter').on('mouseenter', function () {
            const body = $('body');

            if (body.hasClass('page-sidebar-closed') === false || body.hasClass('page-sidebar-fixed') === false || $(this).hasClass('page-sidebar-hovering')) {
                return;
            }

            body.removeClass('page-sidebar-closed').addClass('page-sidebar-hover-on');

            if (body.hasClass('page-sidebar-reversed')) {
                $(this).width(sidebarWidth);
            } else {
                $(this).addClass('page-sidebar-hovering');
                $(this).animate({
                    width: sidebarWidth
                }, 400, '', function () {
                    $(this).removeClass('page-sidebar-hovering');
                });
            }
        });

        $('.page-sidebar').off('mouseleave').on('mouseleave', function () {
            const body = $('body');

            if (body.hasClass('page-sidebar-hover-on') === false || body.hasClass('page-sidebar-fixed') === false || $(this).hasClass('page-sidebar-hovering')) {
                return;
            }

            if (body.hasClass('page-sidebar-reversed')) {
                $('body').addClass('page-sidebar-closed').removeClass('page-sidebar-hover-on');
                $(this).width(sidebarCollapsedWidth);
            } else {
                $(this).addClass('page-sidebar-hovering');
                $(this).animate({
                    width: sidebarCollapsedWidth
                }, 400, '', function () {
                    $('body').addClass('page-sidebar-closed').removeClass('page-sidebar-hover-on');
                    $(this).removeClass('page-sidebar-hovering');
                });
            }
        });
    };

    // Handles sidebar toggler to close/hide the sidebar.
	var handleSidebarToggler = function() {
		const viewport = getViewPort();
		if ($.cookie && $.cookie('sidebar_closed') === '1' && viewport.width >= 992) {
			$('body').addClass('page-sidebar-closed');
		}

		// handle sidebar show/hide
		$('.page-sidebar, .header').on('click',
			'.sidebar-toggler',
			function(e) {
				var body = $('body');
				var sidebar = $('.page-sidebar');

				if ((body.hasClass('page-sidebar-hover-on') && body.hasClass('page-sidebar-fixed')) ||
					sidebar.hasClass('page-sidebar-hovering')) {
					body.removeClass('page-sidebar-hover-on');
					sidebar.css('width', '').hide().show();
					handleSidebarAndContentHeight(); //fix content & sidebar height
					if ($.cookie) {
						$.cookie('sidebar_closed', '0');
					}
					e.stopPropagation();
					runResponsiveHandlers();
					return;
				}

				$('.sidebar-search', sidebar).removeClass('open');

				if (body.hasClass('page-sidebar-closed')) {
					body.removeClass('page-sidebar-closed');
					if (body.hasClass('page-sidebar-fixed')) {
						sidebar.css('width', '');
					}
					if ($.cookie) {
						$.cookie('sidebar_closed', '0');
					}
				} else {
					body.addClass('page-sidebar-closed');
					if ($.cookie) {
						$.cookie('sidebar_closed', '1');
					}
				}
				handleSidebarAndContentHeight(); //fix content & sidebar height
				runResponsiveHandlers();
			});

		// handle the search bar close
		$('.page-sidebar').on('click',
			'.sidebar-search .remove',
			function(e) {
				e.preventDefault();
				$('.sidebar-search').removeClass('open');
			});
	};

    var handleGoTop = function () {
        jQuery('.footer').on('click', '.go-top', function (e) {
            App.scrollTo();
            e.preventDefault();
        });
    };

    // Handles Bootstrap Accordions.
    var handleAccordions = function () {
        jQuery('body').on('shown.bs.collapse', '.accordion.scrollable', function (e) {
            App.scrollTo($(e.target));
        });
    };

    // Handles Bootstrap Tabs.
    var handleTabs = function () {
        // fix content height on tab click
        $('body').on('shown.bs.tab', '.nav.nav-tabs', function () {
            handleSidebarAndContentHeight();
        });

        //activate tab if tab id provided in the URL
        if (location.hash) {
            const tabid = location.hash.substr(1);
            $('a[href="#' + tabid + '"]').parents('.tab-pane:hidden').each(function() {
	            const tabid = $(this).attr('id');
	            $('a[href="#' + tabid + '"]').click();
            });            
            $('a[href="#' + tabid + '"]').click();
        }
	};

	// initialize the layout on page load
	var handleResponsiveOnInit = function() {
		handleSidebarState();
		handleSidebarAndContentHeight();
	};

    // Handles Bootstrap Modals.
	var handleModals = function() {
		// fix stackable modal issue: when 2 or more modals opened, closing one of modal will remove .modal-open class. 
		$('body').on('hide.bs.modal',
			function() {
				if ($('.modal:visible').length > 1 && $('html').hasClass('modal-open') === false) {
					$('html').addClass('modal-open');
				} else if ($('.modal:visible').length <= 1) {
					$('html').removeClass('modal-open');
				}
			});

		$('body').on('show.bs.modal',
			'.modal',
			function() {
				if ($(this).hasClass('modal-scroll')) {
					$('body').addClass('modal-open-noscroll');
				}
			});

		$('body').on('hide.bs.modal',
			'.modal',
			function() {
				$('body').removeClass('modal-open-noscroll');
			});
	};

    // Handles Bootstrap Tooltips.
	var handleTooltips = function() {
		jQuery('.tooltips').tooltip();
	};

    // Handles Bootstrap Dropdowns
	var handleDropdowns = function() {
		$('body').on('click', '.dropdown-menu.hold-on-click', function(e) {
				e.stopPropagation();
		});
	};

    // Handle Hower Dropdowns
	var handleDropdownHover = function() {
		$('[data-hover="dropdown"]').dropdownHover();
	};

	var handleAlerts = function() {
		$('body').on('click',
			'[data-close="alert"]',
			function(e) {
				$(this).parent('.alert').hide();
				e.preventDefault();
			});
	};

    // Handles Bootstrap Popovers

    // last popep popover
    var lastPopedPopover;

	var handlePopovers = function() {
		jQuery('.popovers').popover();

		// close last poped popover

		$(document).on('click.bs.popover.data-api',
			function() {
				if (lastPopedPopover) {
					lastPopedPopover.popover('hide');
				}
			});
	};

    // Handles scrollable contents using jQuery SlimScroll plugin.
	var handleScrollers = function() {
		$('.scroller').each(function() {
			var height;
			if ($(this).attr('data-height')) {
				height = $(this).attr('data-height');
			} else {
				height = $(this).css('height');
			}
			$(this).slimScroll({
				allowPageScroll: true, // allow page scroll when the element scroll is ended
				size: '7px',
				color: $(this).attr('data-handle-color') ? $(this).attr('data-handle-color') : '#bbb',
				railColor: $(this).attr('data-rail-color') ? $(this).attr('data-rail-color') : '#eaeaea',
				position: 'right',
				height: height,
				alwaysVisible: $(this).attr('data-always-visible') === '1' ? true : false,
				railVisible: $(this).attr('data-rail-visible') === '1' ? true : false,
				disableFadeOut: true
			});
		});
	};

    // Handle full screen mode toggle
	var handleFullScreenMode = function() {
		// mozfullscreenerror event handler

		// toggle full screen
		function toggleFullScreen() {
			if (!document.fullscreenElement && // alternative standard method
				!document.mozFullScreenElement &&
				!document.webkitFullscreenElement) { // current working methods
				if (document.documentElement.requestFullscreen) {
					document.documentElement.requestFullscreen();
				} else if (document.documentElement.mozRequestFullScreen) {
					document.documentElement.mozRequestFullScreen();
				} else if (document.documentElement.webkitRequestFullscreen) {
					document.documentElement.webkitRequestFullscreen(Element.ALLOW_KEYBOARD_INPUT);
				}
			} else {
				if (document.cancelFullScreen) {
					document.cancelFullScreen();
				} else if (document.mozCancelFullScreen) {
					document.mozCancelFullScreen();
				} else if (document.webkitCancelFullScreen) {
					document.webkitCancelFullScreen();
				}
			}
		}

		$('#trigger_fullscreen').click(function() {
			toggleFullScreen();
		});
	};

    // Handle Select2 Dropdowns
	var handleSelect2 = function() {
		if (jQuery().select2) {
			$('.select2me').select2({
				placeholder: 'Select',
				allowClear: true
			});
		}
	};

    return {

        //main function to initiate the theme
        init: function () {

            //core handlers
            handleInit(); // initialize core variables
            handleResponsiveOnResize(); // set and handle responsive    
            handleScrollers(); // handles slim scrolling contents 
            handleResponsiveOnInit(); // handler responsive elements on page load

            //layout handlers
            handleFixedSidebar(); // handles fixed sidebar menu
            handleFixedSidebarHoverable(); // handles fixed sidebar on hover effect 
            handleSidebarMenu(); // handles main menu
            handleSidebarToggler(); // handles sidebar hide/show
            handleGoTop(); //handles scroll to top functionality in the footer

            //ui component handlers
            handleSelect2(); // handle custom Select2 dropdowns
            handleAlerts(); //handle closabled alerts
            handleDropdowns(); // handle dropdowns
            handleTabs(); // handle tabs
            handleTooltips(); // handle bootstrap tooltips
            handlePopovers(); // handles bootstrap popovers
            handleAccordions(); //handles accordions 
            handleModals(); // handle modals
            handleFullScreenMode(); // handles full screen
        },

        //main function to initiate core javascript after ajax complete
        initAjax: function () {
            handleScrollers(); // handles slim scrolling contents 
            handleSelect2(); // handle custom Select2 dropdowns
            handleDropdowns(); // handle dropdowns
            handleTooltips(); // handle bootstrap tooltips
            handlePopovers(); // handles bootstrap popovers
            handleAccordions(); //handles accordions 
			handleDropdownHover(); // handles dropdown hover       
        },

        //public function to fix the sidebar and content height accordingly
        fixContentHeight: function () {
            handleSidebarAndContentHeight();
        },

        //public function to remember last opened popover that needs to be closed on click
        setLastPopedPopover: function (el) {
            lastPopedPopover = el;
        },

        //public function to add callback a function which will be called on window resize
        addResponsiveHandler: function (func) {
            responsiveHandlers.push(func);
        },

        // useful function to make equal height for contacts stand side by side
        setEqualHeight: function (els) {
            var tallestEl = 0;
            els = jQuery(els);
            els.each(function () {
	            const currentHeight = $(this).height();
	            if (currentHeight > tallestEl) {
	                tallestEl = currentHeight;
                }
            });
            els.height(tallestEl);
        },

        // wrapper function to scroll(focus) to an element
        scrollTo: function (el, offeset) {
            var pos = el && el.length > 0 ? el.offset().top : 0;

            if (el) {
                if ($('body').hasClass('page-header-fixed')) {
                    pos = pos - $('.header').height(); 
                }            
                pos = pos + (offeset ? offeset : -1 * el.height());
            }

            jQuery('html,body').animate({
                scrollTop: pos
            }, 'slow');
        },

        // function to scroll to the top
        scrollTop: function () {
            App.scrollTo();
        },

        // wrapper function to  block element(indicate loading)
        blockUI: function (options) {
            options = $.extend(true, {}, options);
            var html;
            if (options.iconOnly) {
                html = '<div class="loading-message ' + (options.boxed ? 'loading-message-boxed' : '')+'"><img style="" src="./assets/img/loading-spinner-grey.gif" align=""></div>';
            } else if (options.textOnly) {
                html = '<div class="loading-message ' + (options.boxed ? 'loading-message-boxed' : '')+'"><span>&nbsp;&nbsp;' + (options.message ? options.message : 'LOADING...') + '</span></div>';
            } else {    
                html = '<div class="loading-message ' + (options.boxed ? 'loading-message-boxed' : '')+'"><img style="" src="./assets/img/loading-spinner-grey.gif" align=""><span>&nbsp;&nbsp;' + (options.message ? options.message : 'LOADING...') + '</span></div>';
            }

            if (options.target) { // element blocking
                const el = jQuery(options.target);
                if (el.height() <= $(window).height()) {
                    options.cenrerY = true;
                }            
                el.block({
                    message: html,
                    baseZ: options.zIndex ? options.zIndex : 1000,
                    centerY: options.cenrerY !== undefined ? options.cenrerY : false,
                    css: {
                        top: '10%',
                        border: '0',
                        padding: '0',
                        backgroundColor: 'none'
                    },
                    overlayCSS: {
                        backgroundColor: options.overlayColor ? options.overlayColor : '#000',
                        opacity: options.boxed ? 0.05 : 0.1, 
                        cursor: 'wait'
                    }
                });
            } else { // page blocking
                $.blockUI({
                    message: html,
                    baseZ: options.zIndex ? options.zIndex : 1000,
                    css: {
                        border: '0',
                        padding: '0',
                        backgroundColor: 'none'
                    },
                    overlayCSS: {
                        backgroundColor: options.overlayColor ? options.overlayColor : '#000',
                        opacity: options.boxed ? 0.05 : 0.1,
                        cursor: 'wait'
                    }
                });
            }            
        },

        // wrapper function to  un-block element(finish loading)
        unblockUI: function (target) {
            if (target) {
                jQuery(target).unblock({
                    onUnblock: function () {
                        jQuery(target).css('position', '');
                        jQuery(target).css('zoom', '');
                    }
                });
            } else {
                $.unblockUI();
            }
        },

        startPageLoading: function(message) {
            $('.page-loading').remove();
            $('body').append('<div class="page-loading"><img src="assets/img/loading-spinner-grey.gif"/>&nbsp;&nbsp;<span>' + (message ? message : 'Loading...') + '</span></div>');
        },

        stopPageLoading: function() {
            $('.page-loading').remove();
        },

        //public helper function to get actual input value(used in IE9 and IE8 due to placeholder attribute not supported)
        getActualVal: function (el) {
            el = jQuery(el);
            if (el.val() === el.attr('placeholder')) {
                return '';
            }
            return el.val();
        },

        getUniqueID: function() {
            return 'prefix_' + Math.floor(Math.random() * (new Date()).getTime());
        },

        alert: function(options) {

            options = $.extend(true, {
                container: '', // alerts parent container(by default placed after the page breadcrumbs)
                place: 'append', // append or prepent in container 
                type: 'success',  // alert's type
                message: '',  // alert's message
                close: true, // make alert closable
                reset: true, // close all previouse alerts first
                focus: true, // auto scroll to the alert after shown
                closeInSeconds: 0, // auto close after defined seconds
                icon: '' // put icon before the message
            }, options);

            var id = App.getUniqueID('app_alert');

			const html = '<div id="' + id + '" class="app-alerts alert alert-' + options.type + ' fade in">' +
				(options.close ? '<button type="button" class="close" data-dismiss="alert" aria-hidden="true"></button>' : '') +
				(options.icon !== '' ? '<i class="fa-lg fa fa-' + options.icon + '"></i>  ' : '') +
				options.message + '</div>';

            if (options.reset) {
                $('.app-alerts').remove();
            }

            if (!options.container) {
                $('.page-breadcrumb').after(html);
            } else {
                if (options.place === 'append') {
                    $(options.container).append(html);
                } else {
                    $(options.container).prepend(html);
                }
            }

            if (options.focus) {
                App.scrollTo($('#' + id));
            }

            if (options.closeInSeconds > 0) {
                setTimeout(function(){
                    $('#' + id).remove();
                }, options.closeInSeconds * 1000);
            }
        }
    };
}();

function toggleUsageExpansion(productId, sender) {
	sender = $(sender);
	if (sender.hasClass('collapse-icon')) {
		sender.removeClass('collapse-icon');
		sender.addClass('expand-icon');
		sender.attr('src', '/images/expand.png');
		$('.row-' + productId).hide();
	} else {
		sender.removeClass('expand-icon');
		sender.addClass('collapse-icon');
		sender.attr('src', '/images/collapse.png');
		$('.row-' + productId).show();
	}
}

$(document).on('submit', function (event) {
	var btn = $(this).find("button[type=submit]:focus");
	if (btn.length == 0) {
		btn = $(this).find("input[type=submit]:focus");
		btn.val("Please wait....");
	}
	btn.prop("disabled", true);
	btn.text("Please wait....");
});

function initializeDatePicker(datePickerId, dowString, dowHighlight) {
	$(datePickerId).datepicker({
		daysOfWeekDisabled: dowString,
		daysOfWeekHighlighted: dowHighlight,
		autoclose: true,
		assumeNearbyYear: true,
		forceParse: false,
		format: 'yyyy-mm-dd',
		startDate: new Date(),
		weekStart: 1
	} );
}