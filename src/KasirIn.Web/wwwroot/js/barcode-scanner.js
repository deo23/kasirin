window.KasirInScanner = {
    html5Qrcode: null,
    audioCtx: null,
    playBeep: function () {
        try {
            if (!this.audioCtx) {
                this.audioCtx = new (window.AudioContext || window.webkitAudioContext)();
            }
            if (this.audioCtx.state === "suspended") {
                this.audioCtx.resume();
            }
            var osc = this.audioCtx.createOscillator();
            var gain = this.audioCtx.createGain();
            osc.type = "sine";
            osc.frequency.value = 800;
            gain.gain.value = 0.15;
            osc.connect(gain);
            gain.connect(this.audioCtx.destination);
            osc.start();
            osc.stop(this.audioCtx.currentTime + 0.15);
        } catch (e) {
            console.log("Audio feedback error:", e);
        }
    },
    startScan: function (dotNetHelper, elementId) {
        var self = this;
        if (self.html5Qrcode) {
            self.stopScan();
        }
        try {
            self.html5Qrcode = new Html5Qrcode(elementId);
            self.html5Qrcode.start(
                { facingMode: "environment" },
                { fps: 10, qrbox: { width: 250, height: 250 } },
                function (decodedText) {
                    self.playBeep();
                    dotNetHelper.invokeMethodAsync("OnBarcodeScanned", decodedText);
                    self.stopScan();
                },
                function (errorMessage) {
                    // Ignore frame scan failures
                }
            ).catch(function (err) {
                console.error("Camera access error:", err);
            });
        } catch (err) {
            console.error("Scanner init error:", err);
        }
    },
    stopScan: function () {
        if (this.html5Qrcode) {
            try {
                this.html5Qrcode.stop().then(function () {
                    this.html5Qrcode.clear();
                    this.html5Qrcode = null;
                }).catch(function () {
                    this.html5Qrcode = null;
                });
            } catch (e) {
                this.html5Qrcode = null;
            }
        }
    }
};
