(function ($) {
    var options = {
        series: { stack: null } // or number/string
    };

    function init(plot) {

        // will be built up dynamically as a hash from x-value to
        var stackBases = {};

        function findMatchingSeries(s, allseries) {
            var res = null
            for (var i = 0; i < allseries.length; ++i) {
                if (s == allseries[i])
                    break;

                if (allseries[i].stack == s.stack)
                    res = allseries[i];
            }

            return res;
        }

        function stackData(plot, s, datapoints) {
            if (s.stack == null)
                return;

            var newPoints = [];

            for (var i=0; i <  datapoints.points.length; i += 3) {

                if (!stackBases[datapoints.points[i]]) {
                    stackBases[datapoints.points[i]] = 0;
                }

                // note that the values need to be turned into absolute y-values.
                // in other words, if you were to stack (x, y1), (x, y2), and (x, y3),
                // (each from different series, which is where stackBases comes in),
                // you'd want the new points to be (x, y1, 0), (x, y1+y2, y1), (x, y1+y2+y3, y1+y2)
                // generally, (x, thisValue + (base up to this point), (base up to this point))

                newPoints[i] = datapoints.points[i];
                newPoints[i+1] = datapoints.points[i+1] + stackBases[datapoints.points[i]];
                newPoints[i+2] = stackBases[datapoints.points[i]];

                stackBases[datapoints.points[i]] += datapoints.points[i+1];

            }

            datapoints.points = newPoints;
        }

        plot.hooks.processDatapoints.push(stackData);
    }

    $.plot.plugins.push({
        init: init,
        options: options,
        name: 'stack',
        version: '1.0'
    });
})(jQuery);