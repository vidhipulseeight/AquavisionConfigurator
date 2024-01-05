var popupDefault = {
	width: '150px',
	top: 10,
	left: 80,
	display: 'block'
};

var popupDistance = 10;

$(function () {
	$('.bubbleInfo').each(function () {
		// options
		var time = 150;
		var hideDelay = 100;
		var hideDelayTimer = null;
		var myPopupDistance = ($(this).hasClass('customPopup') ? (customPopupDistance || popupDistance) : popupDistance);
		var myPopupCSS = ($(this).hasClass('customPopup') ? (customPopup || popupDefault) : popupDefault);
		// tracker
		var beingShown = false;
		var shown = false;

		var trigger = $('.trigger', this).css('cursor', 'pointer');
		var popup = $('.popup', this).css('opacity', 0);

		// set the mouseover and mouseout on both element
		$([trigger.get(0), popup.get(0)]).mouseover(function () {
			// stops the hide event if we move from the trigger to the popup element
			if (hideDelayTimer) clearTimeout(hideDelayTimer);

			// don't trigger the animation again if we're being shown, or already visible
			if (beingShown || shown) {
				return;
			} else {
				beingShown = true;
				// reset position of popup box
				popup.css(myPopupCSS)
					// (we're using chaining on the popup) now animate it's opacity and position
				.animate({
					top: '-=' + myPopupDistance + 'px',
					opacity: 1
				}, time, 'swing', function () {
					// once the animation is complete, set the tracker variables
					beingShown = false;
					shown = true;
				});
			}
		}).mouseout(function () {
			// reset the timer if we get fired again - avoids double animations
			if (hideDelayTimer) clearTimeout(hideDelayTimer);

			// store the timer so that it can be cleared in the mouseover if required
			hideDelayTimer = setTimeout(function () {
				hideDelayTimer = null;
				popup.animate({
					top: '-=' + myPopupDistance + 'px',
					opacity: 0
				}, time, 'swing', function () {
					// once the animate is complete, set the tracker variables
					shown = false;
					// hide the popup entirely after the effect (opacity alone doesn't do the job)
					popup.css('display', 'none');
				});
			}, hideDelay);
		});
	});
});