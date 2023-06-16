Chart.plugins.register({
    id: 'centerText',
    afterDraw: function (chart) {
        var ctx = chart.ctx;
        var width = chart.chart.width;
        var height = chart.chart.height;
        var valueFontSize = (height / 5).toFixed(2); // Adjust the divisor to increase or decrease the font size of the value
        var unitFontSize = (height / 7).toFixed(2); // Adjust the divisor to increase or decrease the font size of the unit
        var value = chart.data.datasets[0].data[0].toFixed(2);
        var unitText = 'Bar';

        ctx.font = "bold " + valueFontSize + "px Arial"; // Set the font style and size for the value
        ctx.textBaseline = "middle";
        ctx.fillStyle = "#36b9cc";
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';
        ctx.fillText(value, width / 2, height / 1.6);
        ctx.font = unitFontSize + "px Arial"; // Set the font style and size for the unit
        ctx.fillStyle = "#6c757d"; // Set the text color for the unit

        ctx.fillText(unitText, width / 2, height / 1.2); // Adjust the vertical position of the unit
    }
});