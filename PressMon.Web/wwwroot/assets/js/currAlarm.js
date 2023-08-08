var baseUrl = rootUrl + "Home/";
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
            url: baseUrl + "GetAlarm",
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                var value = data.value;
                var alarms = data.alarms;
                var valM01 = value[0].pressure;
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
                    } else if (item.info === 'HH') {
                        valHH = item.value;
                    }
                });
                if (valM01 >= valH) {
                    if (valM01 >= valHH) {
                        press.textContent = "PT-01 HH : " + valM01.toString() + " Bar";
                    } else {
                        press.textContent = "PT-01 H : " + valM01.toString() + " Bar";
                    }
                    blinkText();
                } else if (valM01 <= valL ) {
                    if (valM01 <= valLL) {
                        press.textContent = "PT-01 LL : " + valM01.toString() + " Bar";
                    } else {
                        press.textContent = "PT-01 L : " + valM01.toString() + " Bar";
                    }
                    blinkText();
                } else {
                    Disabled();
                }

                if (valM02 >= valH) {
                    if (valM02 >= valHH) {
                        press.textContent = "PT-02 HH : " + valM02.toString() + " Bar";
                    } else {
                        press.textContent = "PT-02 H : " + valM02.toString() + " Bar";
                    }
                    blinkText();
                } else if (valM02 <= valL) {
                    if (valM02 <= valLL) {
                        press.textContent = "PT-02 LL : " + valM02.toString() + " Bar";
                    } else {
                        press.textContent = "PT-02 L : " + valM02.toString() + " Bar";
                    }
                    blinkText();
                } else {
                    Disabled();
                }

            },
            error: function (error) {
                console.log(error);
            }
        });
    }
    blinkInterval = setInterval(SettingAlarm, visibleDuration + hiddenDuration); // Start the initial blinking interval
});