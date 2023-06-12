$(function () {

    $("#thongke").click(function () {
        var nameValue = document.getElementById('reservationtime').value;

        var checkDay = document.getElementById('statisticalDay').checked;
        var checkMonth = document.getElementById('statisticalMonth').checked;
        var checkYear = document.getElementById('statisticalYear').checked;

        var url = "";

        if (checkYear) {
            date1 = "01/01/" + nameValue.substring(6, 10);
            date2 = "01/12/" + nameValue.substring(19, 23);
            url = "/Account/GetStatisticalYear";
        }
        else if (checkMonth) {
            date1 = "01/" + nameValue.substring(3, 10);
            date2 = "01/" + nameValue.substring(16, 23);
            url = "/Account/GetStatisticalMonth";
        }
        else if (checkDay){
            date1 = nameValue.substring(0, 10);
            date2 = nameValue.substring(13, 23);
            url = "/Account/GetStatisticalDay";
        }

        apiRequest(url, date1, date2);

    });

})

function apiRequest(url, date1, date2) {
    var arrDate = [];
    var arrDoanhThu = [];
    var arrLoiNhuan = [];
    $.ajax({
        url: url,
        data: { dateFrom: date1, dateTo: date2},
        type: 'GET',
        contentType: 'application/json',
        success: function (rs) {
            $.each(rs.result, function (i, item) {
                arrDate.push(item.date);
                arrDoanhThu.push(item.doanhThu);
                arrLoiNhuan.push(item.loiNhuan);

            })

            var areaChartData = {
                labels: arrDate,
                datasets: [
                    {
                        label: 'Lợi nhuận',
                        backgroundColor: 'rgba(60,141,188,0.9)',
                        borderColor: 'rgba(60,141,188,0.8)',
                        pointRadius: false,
                        pointColor: '#3b8bba',
                        pointStrokeColor: 'rgba(60,141,188,1)',
                        pointHighlightFill: '#fff',
                        pointHighlightStroke: 'rgba(60,141,188,1)',
                        data: arrLoiNhuan
                    },
                    {
                        label: 'Danh thu',
                        backgroundColor: 'rgba(210, 214, 222, 1)',
                        borderColor: 'rgba(210, 214, 222, 1)',
                        pointRadius: false,
                        pointColor: 'rgba(210, 214, 222, 1)',
                        pointStrokeColor: '#c1c7d1',
                        pointHighlightFill: '#fff',
                        pointHighlightStroke: 'rgba(220,220,220,1)',
                        data: arrDoanhThu
                    },
                ]
            }

            var barChartCanvas = $('#barChart').get(0).getContext('2d')
            var barChartData = $.extend(true, {}, areaChartData)
            var temp0 = areaChartData.datasets[0]
            var temp1 = areaChartData.datasets[1]
            barChartData.datasets[0] = temp1
            barChartData.datasets[1] = temp0

            var barChartOptions = {
                responsive: true,
                maintainAspectRatio: false,
                datasetFill: false
            }

            new Chart(barChartCanvas, {
                type: 'bar',
                data: barChartData,
                options: barChartOptions
            })
            load_data(rs.result, rs.Amount_Buy, rs.Amount_Sel);
        }
    });

}

function load_data(data, Amount_Buy, Amount_Sel) {
    var html = "";
    $.each(data, function (i, item) {
        html += "<tr>";
        html += "<td>" + (i + 1) + "</td>";
        html += "<td>" + (item.date) + "</td>";
        html += "<td>" + (item.doanhThu) + "</td>";
        html += "<td>" + (item.loiNhuan) + "</td>";
        html += "</tr>";
    });
    html +="<tr>";
    html += "<td>##</td>";
    html += "<td>Tổng</td>";
    html += "<td>" + (Amount_Buy) + "</td>";
    html += "<td>" + (Amount_Sel) + "</td>";
    html += "</tr>";
    $('#load_data').html(html);
}