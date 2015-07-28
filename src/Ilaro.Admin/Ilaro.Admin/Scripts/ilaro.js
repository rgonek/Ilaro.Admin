$(function () {
	$('.date-time-picker').datetimepicker();

	$('.date-picker').datetimepicker({
		pickTime: false
	});

	$('.time-picker').datetimepicker({
		pickDate: false
	});

	$('.numeric').spinedit({ value: 0 });

	$('select:not(.dual-list)').chosen({ disable_search_threshold: 10 });

	$('textarea.markdown').markdown();

	$('textarea.html').summernote({ height: 100 });

	$('.dual-list').bootstrapDualListbox({
	    moveOnSelect: false,
	    nonSelectedListLabel: 'Non-selected',
        selectedListLabel: 'Selected'
	});

	$('input[type=file]').bootstrapFileInput();

	$('th > span').tooltip();
	$('.open-modal').click(function (e) {
		e.preventDefault();
		$('#modal-image').attr('src', $(this).attr('href'));
		$('#modal').modal();
		return false;
	});
	$('.autoPostBack').change(function () {
		$(this).parents('form').submit();
	});
	$('legend .btn').click(function () {
		var $this = $(this);
		var $i = $this.children('i');
		if ($i.hasClass('glyphicon glyphicon-plus')) {
			$i.removeClass('glyphicon glyphicon-plus');
			$i.addClass('glyphicon glyphicon-minus');
			$this.parent().siblings('.fields').slideDown();
		}
		else {
			$i.removeClass('glyphicon glyphicon-minus');
			$i.addClass('glyphicon glyphicon-plus');
			$this.parent().siblings('.fields').slideUp();
		}
	});

	$('.create-foreign').click(function (e) {
	    //e.preventDefault();

	    //var url = $(this).attr('href');

	    //$.get(url, function (data) {
	    //	$('#foreign-modal .modal-body').html(data);
	    //	$('#foreign-modal').modal('show');
	    //});
	});

	$('[data-role=delete-image]').click(function (e) {
	    var $this = $(this);
	    $this.siblings('input[type=hidden]').val(true);
	    var $file = $this.parent().find('input[type=file]');
	    $file.wrap('<form>').closest('form').get(0).reset();
	    $file.unwrap();
	    $this.siblings('.file-input-name').remove();
	    $this.parents('.form-group').find('[data-role=image]').remove();
	});

	$('.date-filter-custom-range').click(function (e) {
	    e.preventDefault();
	    var $this = $(this);
	    var $container = $this.parent();
	    var from = $container.siblings('.date-from').val().replace(/-/gi, '.');
	    var to = $container.siblings('.date-to').val().replace(/-/gi, '.');
	    var range = from + '-' + to;
	    var href = $this.attr('href').replace('date-range', range);
	    window.location.href = href;
	});

	$('.string-filter-custom').click(function (e) {
	    e.preventDefault();
	    var $this = $(this);
	    var $container = $this.parent();
	    var search = $container.siblings('input[type=text]').val();
	    var href = $this.attr('href').replace('custom-string', search);
	    window.location.href = href;
	});
});