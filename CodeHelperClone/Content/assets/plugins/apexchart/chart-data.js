'use strict';

$(document).ready(function() {
	let incomeArr = [0];
	$.ajax({
		url: "/instructor/getEarningChartData",
		type: "get",
		success: function (res) { 
			var date = new Date();
			
			let largMonth = date.getMonth()+1;
			res.forEach((item, index) => {
				for (let i = 1; i <= largMonth; i++) {
					if (index % 2 != 0) {
						if (i == res[index]) {
							incomeArr.push(res[index-1]);
						}
						else {
							incomeArr.push(0);
						}
					}
				}
			})

			
			
			if ($('#instructor_chart').length > 0) {


				var options = {
					series: [{
						name: "Current month",
						data: incomeArr
					},
					],
					colors: ['#FF9364'],
					chart: {
						height: 300,
						type: 'area',
						toolbar: {
							show: false
						},
						zoom: {
							enabled: false
						}
					},
					markers: {
						size: 3,
					},
					dataLabels: {
						enabled: false
					},
					stroke: {
						curve: 'smooth',
						width: 3,
					},
					legend: {
						position: 'top',
						horizontalAlign: 'right',
					},
					grid: {
						show: false,
					},
					yaxis: {
						axisBorder: {
							show: true,
						},
					},
					xaxis: {
						categories: ['', 'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
					}
				};

				var chart = new ApexCharts(document.querySelector("#instructor_chart"), options);
				chart.render();
			}

		},
		error: function () {

		}
	})
	function generateData(baseval, count, yrange) {
		var i = 0;
		var series = [];
		while (i < count) {
			var x = Math.floor(Math.random() * (750 - 1 + 1)) + 1;;
			var y = Math.floor(Math.random() * (yrange.max - yrange.min + 1)) + yrange.min;
			var z = Math.floor(Math.random() * (75 - 15 + 1)) + 15;

			series.push([x, y, z]);
			baseval += 86400000;
			i++;
		}
		return series;
	}
	
	// Chart

	let monthArr = [];
	let orderArr = [];
	$.ajax({
		url: "/Instructor/GetOrderChartData",
		type: "get",
		success: function (res) {
			debugger;
			let largeMonth = res[res.length-1]
			for (let i = 0; i <= largeMonth; i++) {
				const date = new Date(2009, i, 10);  // 2009-11-10
				const month = date.toLocaleString('default', { month: 'short' });
				monthArr[i] = month;
				res.forEach((item, index) => {
					if (res[index] == i && index % 2 != 0) {
						orderArr[i-1] = res[index - 1];
					}
					else {
						orderArr[i] = 0;
					}
				})
			}
			console.log(orderArr)
			if ($('#order_chart').length > 0) {
				var sCol = {
					chart: {
						height: 350,
						type: 'bar',
						toolbar: {
							show: false,
						}
					},
					plotOptions: {
						bar: {
							horizontal: false,
							columnWidth: '20%',
							endingShape: 'rounded',
							startingShape: 'rounded'
						},
					},
					colors: ['#1D9CFD'],
					dataLabels: {
						enabled: false
					},
					stroke: {
						show: true,
						width: 2,
						colors: ['transparent']
					},
					series: [{
						name: 'Revenue',
						data: orderArr
					}],
					xaxis: {
						categories: monthArr,
					},
					fill: {
						opacity: 1

					},
					tooltip: {
						y: {
							formatter: function (val) {
								return "₹" + val
							}
						}
					}
				}

				var chart = new ApexCharts(
					document.querySelector("#order_chart"),
					sCol
				);

				chart.render();
			}
		}
	});
	// Simple Column
	
});