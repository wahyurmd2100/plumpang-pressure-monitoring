var baseUrl = document.getElementById("baseUrlData").dataset.baseUrl;
$(document).ready(function () {
    var blinkingText = document.getElementById("blinkingText");
    var press = document.getElementById("press-value");
    var sound = document.getElementById("sound");
    var visibleDuration = sound.duration * 1000; // Duration in milliseconds for the visible state
    var hiddenDuration = visibleDuration / 2; // Duration in milliseconds for the hidden state
    var blinkInterval; // Variable to store the interval ID


    function blinkText() {
        if (blinkingText.style.visibility === "" || blinkingText.style.visibility === "visible") {
            blinkingText.style.visibility = "hidden";
            setTimeout(function () {
                blinkingText.style.visibility = "visible";
                sound.play();
            }, hiddenDuration);
        } else {
            blinkingText.style.visibility = "visible";
            setTimeout(function () {
                blinkingText.style.visibility = "hidden";
                sound.pause();
                //sound.currentTime = 0;
            }, visibleDuration);
        }
    }
    function Disabled() {
        blinkingText.style.visibility = "hidden";
        sound.pause();
        sound.currentTime = 0;
    }
    function SettingAlarm() {
        $.ajax({
            url: baseUrl,
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                var value = data.value;
                var alarms = data.alarms;
                var valM01 = value[0];
                var valM02 = value[1].pressure;
                var valLL;
                var valL;
                var valH;
                var valHH;
                alarms.forEach(function (item) {
                    if (item.info === 'LL') {
                        valLL = item.value;
                    } else if (item.info === 'L') {
                        valL = item.value;
                    } else if (item.info === 'H') {
                        valH = item.value;
                        if (valM02 >= valH) {
                            press.textContent = item.info.toString() + " :" + valM02.toString() + " Bar";
                            blinkText();
                        } else {
                            Disabled();
                        }
                    } else if (item.info === 'HH') {
                        valHH = item.value;
                    }


                });
                //console.log(valM02);





                //data.alarms.forEach(function (item) {
                //    //if (item.info == 'H' && ala)
                //    //{
                //    //    press.textContent = item.info.toString() + " :" + value.toString() + " Bar";
                //    //    if (value > item.value) {
                //    //        blinkText();
                //    //    }
                //    //    else {
                //    //        Disabled();
                //    //    }
                //    //}
                //});

            },
            error: function (error) {
                console.log(baseUrl);
                //console.log(error);
            }
        });
    }
    blinkInterval = setInterval(SettingAlarm, visibleDuration + hiddenDuration); // Start the initial blinking interval
});