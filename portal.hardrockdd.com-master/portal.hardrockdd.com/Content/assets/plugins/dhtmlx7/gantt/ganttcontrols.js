function toggleMode(toggle) {
	gantt.$zoomToFit = !gantt.$zoomToFit;
	if (gantt.$zoomToFit) {
		toggle.innerHTML = "Set default Scale";
		//Saving previous scale state for future restore
		saveConfig();
		zoomToFit();
	} else {

		toggle.innerHTML = "Zoom to Fit";
		//Restore previous scale state
		restoreConfig();
		gantt.render();
	}
}

var cachedSettings = {};

function saveConfig() {
	var config = gantt.config;
	cachedSettings = {};
	cachedSettings.scales = config.scales;
	cachedSettings.start_date = config.start_date;
	cachedSettings.end_date = config.end_date;
	cachedSettings.scroll_position = gantt.getScrollState();
}

function restoreConfig() {
	applyConfig(cachedSettings);
}

function applyConfig(config, dates) {
	gantt.config.scales = config.scales;
	var lowest_scale = config.scales.reverse()[0];

	if (dates && dates.start_date && dates.end_date) {
		gantt.config.start_date = gantt.date.add(dates.start_date, -1, lowest_scale.unit);
		gantt.config.end_date = gantt.date.add(gantt.date[lowest_scale.unit + "_start"](dates.end_date), 2, lowest_scale.unit);
	} else {
		gantt.config.start_date = gantt.config.end_date = null;
	}

	// restore the previous scroll position
	if (config.scroll_position) {
		setTimeout(function () {
			gantt.scrollTo(config.scroll_position.x, config.scroll_position.y)
		}, 4)
	}
}


function zoomToFit() {
	//console.log('zoomToFit');
	var project = gantt.getSubtaskDates(),
		areaWidth = gantt.$task.offsetWidth,
		scaleConfigs = zoomConfig.levels;
	//console.log('zoomConfig.levels', zoomConfig.levels);
	for (var i = 0; i < scaleConfigs.length; i++) {

		var columnCount = getUnitsBetween(project.start_date, project.end_date, scaleConfigs[i].scales[scaleConfigs[i].scales.length - 1].unit, scaleConfigs[i].scales[0].step);
		//console.log('columnCount', columnCount);
		if ((columnCount + 2) * gantt.config.min_column_width <= areaWidth) {
			break;
		}
	}


	if (i == scaleConfigs.length) {
		i--;
	}

	gantt.ext.zoom.setLevel(scaleConfigs[i].name);
	applyConfig(scaleConfigs[i], project);
}

// get number of columns in timeline
function getUnitsBetween(from, to, unit, step) {
	var start = new Date(from),
		end = new Date(to);
	var units = 0;
	while (start.valueOf() < end.valueOf()) {
		units++;
		start = gantt.date.add(start, step, unit);
	}
	return units;
}

function zoom_in() {
	gantt.ext.zoom.zoomIn();
	gantt.$zoomToFit = false;
	document.querySelector(".zoom_toggle").innerHTML = "Zoom to Fit";
}
function zoom_out() {
	gantt.ext.zoom.zoomOut();
	gantt.$zoomToFit = false;
	document.querySelector(".zoom_toggle").innerHTML = "Zoom to Fit";
}


var zoomConfig2 = {
	levels: [
		// hours
		{
			name: "hour",
			scale_height: 27,
			scales: [
				{ unit: "day", step: 1, format: "%d %M" },
				{ unit: "hour", step: 1, format: "%H:%i" },
			]
		},
		// days
		{
			name: "day",
			scale_height: 27,
			scales: [
				{ unit: "day", step: 1, format: "%d %M" }
			]
		},
		// weeks
		{
			name: "week",
			scale_height: 50,
			scales: [
				{
					unit: "week", step: 1, format: function (date) {
						var dateToStr = gantt.date.date_to_str("%d %M");
						var endDate = gantt.date.add(date, -6, "day");
						var weekNum = gantt.date.date_to_str("%W")(date);
						return "#" + weekNum + ", " + dateToStr(date) + " - " + dateToStr(endDate);
					}
				},
				{ unit: "day", step: 1, format: "%j %D" }
			]
		},
		// months
		{
			name: "month",
			scale_height: 50,
			scales: [
				{ unit: "month", step: 1, format: "%F, %Y" },
				{
					unit: "week", step: 1, format: function (date) {
						var dateToStr = gantt.date.date_to_str("%d %M");
						var endDate = gantt.date.add(gantt.date.add(date, 1, "week"), -1, "day");
						return dateToStr(date) + " - " + dateToStr(endDate);
					}
				}
			]
		},
		// quarters
		{
			name: "quarter",
			height: 50,
			scales: [
				{
					unit: "quarter", step: 3, format: function (date) {
						var dateToStr = gantt.date.date_to_str("%M %y");
						var endDate = gantt.date.add(gantt.date.add(date, 3, "month"), -1, "day");
						return dateToStr(date) + " - " + dateToStr(endDate);
					}
				},
				{ unit: "month", step: 1, format: "%M" },
			]
		},
		// years
		{
			name: "year",
			scale_height: 50,
			scales: [
				{
					unit: "year", step: 5, format: function (date) {
						var dateToStr = gantt.date.date_to_str("%Y");
						var endDate = gantt.date.add(gantt.date.add(date, 5, "year"), -1, "day");
						return dateToStr(date) + " - " + dateToStr(endDate);
					}
				}
			]
		},
		// decades
		{
			name: "decades",
			scale_height: 50,
			scales: [
				{
					unit: "year", step: 100, format: function (date) {
						var dateToStr = gantt.date.date_to_str("%Y");
						var endDate = gantt.date.add(gantt.date.add(date, 100, "year"), -1, "day");
						return dateToStr(date) + " - " + dateToStr(endDate);
					}
				},
				{
					unit: "year", step: 10, format: function (date) {
						var dateToStr = gantt.date.date_to_str("%Y");
						var endDate = gantt.date.add(gantt.date.add(date, 10, "year"), -1, "day");
						return dateToStr(date) + " - " + dateToStr(endDate);
					}
				},
			]
		},
	],
	element: function () {
		return gantt.$root.querySelector(".gantt_task");
	}
};

var zoomConfig = {
	levels: [
		{
			name: "hour",
			scale_height: 60,
			min_column_width: 5,
			scales: [
				{
					unit: "week", step: 1, format: function (date) {
						var dateToStr = gantt.date.date_to_str("%M %d");
						var endDate = gantt.date.add(date, 6, "day");
						var weekNum = gantt.date.date_to_str("%W")(date);//"#" + weekNum + ", " +
						var yearNum = gantt.date.date_to_str("%Y")(date);
						return dateToStr(date) + " - " + dateToStr(endDate) + " " + yearNum;
					}
				},
				{ unit: "day", step: 1, format: "%d, %l" },
				{ unit: "hour", step: 1, format: "%H" },
			]
		},
		{
			name: "day",
			scale_height: 27,
			min_column_width: 80,
			scales: [
				{ unit: "day", step: 1, format: "%d %M" }
			]
		},
		{
			name: "week",
			scale_height: 50,
			min_column_width: 50,
			scales: [
				{
					unit: "week", step: 1, format: function (date) {
						//var dateToStr = gantt.date.date_to_str("%d %M");
						//var endDate = gantt.date.add(date, 6, "day");
						//var weekNum = gantt.date.date_to_str("%W")(date);
						//return "#" + weekNum + ", " + dateToStr(date) + " - " + dateToStr(endDate);

						var dateToStr = gantt.date.date_to_str("%M %d");
						var endDate = gantt.date.add(date, 6, "day");
						var weekNum = gantt.date.date_to_str("%W")(date);//"#" + weekNum + ", " +
						var yearNum = gantt.date.date_to_str("%Y")(date);
						return dateToStr(date) + " - " + dateToStr(endDate) + " " + yearNum;
					}
				},
				{ unit: "day", step: 1, format: "%j %D" }
			]
		},
		{
			name: "month",
			scale_height: 50,
			min_column_width: 120,
			scales: [
				{ unit: "month", format: "%F, %Y" },
				{ unit: "week", format: "Week #%W" }
			]
		},
		{
			name: "quarter",
			height: 50,
			min_column_width: 90,
			scales: [
				{ unit: "month", step: 1, format: "%M" },
				{
					unit: "quarter", step: 1, format: function (date) {
						var dateToStr = gantt.date.date_to_str("%M");
						var endDate = gantt.date.add(gantt.date.add(date, 3, "month"), -1, "day");
						return dateToStr(date) + " - " + dateToStr(endDate);
					}
				}
			]
		},
		{
			name: "year",
			scale_height: 50,
			min_column_width: 30,
			scales: [
				{ unit: "year", step: 1, format: "%Y" }
			]
		}
	],
	element: function () {
		return gantt.$root.querySelector(".gantt_task");
	}
};
(function () {
	function defineMultiselect(gantt) {
		gantt.form_blocks["multiselect"] = {

			render: function (sns) {
				var height = (sns.height || "23") + "px";
				var html = "<div class='gantt_cal_ltext gantt_cal_chosen gantt_cal_multiselect' style='height:" + height + ";'>";
				html += "<select data-placeholder='...' class='chosen-select' multiple>";
				sns.options.forEach(function (option) {
					if (sns.unassigned_value !== undefined && option.id == sns.unassigned_value) {

					}
					else {
						html += "<option value='" + option.id + "'>" + option.text + "</option>";
                    }
				});
				html += "</select>";
				html += "</div>";
				return html;
			},

			set_value: function (node, value, ev, sns) {
				node.style.overflow = "visible";
				node.parentNode.style.overflow = "visible";
				node.style.display = "inline-block";
				var select = $(node.firstChild);
				var currentValue = [];
				value.forEach(function (opt) {
					currentValue.push(opt.resource_id);
				});
				select.val(currentValue);

				select.chosen();
				if (sns.onchange) {
					select.change(function () {
						sns.onchange.call(this);
					})
				}
				select.trigger('chosen:updated');
				select.trigger("change");
			},

			get_value: function (node, ev) {
				var value = $(node.firstChild).val();
				return value;
			},

			focus: function (node) {
				$(node.firstChild).focus();
			}
		};

	}

	if (window.Gantt) {
		window.Gantt.plugin(defineMultiselect);
	} else {
		defineMultiselect(window.gantt);
	}

})();