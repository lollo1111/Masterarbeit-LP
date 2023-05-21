<template>
    <the-card>
        <template #header>Simulation definieren</template>
        <template #default>
            <form @submit.prevent>
                <div class="form-group">
                    <label for="name">Simulation Name <span class="smaller">(überschreibt bereits existierende Simulationen
                            mit demselben Namen)</span></label>
                    <input id="name" type="text" v-model="name">
                </div>
                <div class="form-group">
                    <label for="product">Was für ein Produkt soll produziert werden?</label>
                    <select id="product" v-model="product">
                        <option value="" disabled>Produktart auswählen</option>
                        <option value="schreibtisch">Schreibtisch</option>
                        <option value="schrank">Schrank</option>
                    </select>
                </div>
                <div v-if="product === 'schreibtisch'" class="schreibtisch">
                    <div class="form-group">
                        <label for="style">Wähle einen Tisch-Stil aus</label>
                        <select id="style" v-model="tableStyle">
                            <option value="" disabled>Stil auswählen</option>
                            <option value="modern">Modern</option>
                            <option value="classic">Klassisch</option>
                        </select>
                    </div>
                </div>
                <div v-if="product === 'schrank'" class="schrank">
                    <div class="form-group">
                        <label for="doorType">Wähle eine Türart aus</label>
                        <select id="doorType" v-model="doorType">
                            <option value="" disabled>Stil auswählen</option>
                            <option value="defaultDoor">Default</option>
                            <option value="slidingDoor">Schiebetür</option>
                        </select>
                    </div>
                    <div class="form-group">
                        <label for="mirrorShape">Wähle eine Spiegelform aus</label>
                        <select id="mirrorShape" v-model="mirrorShape">
                            <option value="" disabled>Spiegelform auswählen</option>
                            <option value="circular">Rund</option>
                            <option value="angular">Eckig</option>
                        </select>
                    </div>
                </div>
                <div class="form-group">
                    <label for="min">Paket abhängig von <span class="underline">Höhe</span> für Versand
                        kategorisieren:</label>
                    <div class="range-slider">
                        <div class="slider-track"></div>
                        <div class="border">Die Grenze liegt bei >= {{ maxHeight * 10 }} cm</div>
                        <input class="lower" type="range" id="min" name="min" min="0" max="100" unit="📡"
                            v-model="maxHeight">
                        <div class="boxes">
                            <div class="bubble" :style="smallBox">Kleines Paket</div>
                            <div class="bubble" :style="bigBox">Großes Paket</div>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <label for="min">Paket abhängig von <span class="underline">Gewicht</span> für Versand
                        kategorisieren:</label>
                    <div class="range-slider">
                        <div class="slider-track"></div>
                        <div class="border">Die Grenze liegt bei >= {{ maxWeight * 10 }} g</div>
                        <input class="lower" type="range" id="wmin" name="min" min="0" max="100" v-model="maxWeight">
                        <div class="boxes">
                            <div class="bubble" :style="wsmallBox">Kleines Paket</div>
                            <div class="bubble" :style="wbigBox">Großes Paket</div>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <input id="zusatz" type="checkbox" v-model="additionalEquipment">
                    <label class="inlineLabel" for="zusatz">Zusatzausrüstung <span class="smaller">(optional)</span></label>
                </div>
                <div class="form-group">
                    <input id="express" type="checkbox" v-model="express">
                    <label class="inlineLabel" for="express">Expresslieferung <span class="smaller">(optional)</span></label>
                </div>
                <div class="form-group last">
                    <label for="min">Paketgrößen festlegen:</label>
                    <div class="sizes">
                        <div class="sizeS">
                            <h4><span class="underline">Box S</span></h4>
                            <input id="prodSchrank" type="checkbox" v-model="sBoxSchrank">
                            <label for="prodSchrank">Schrank</label><br>
                            <input id="additionalSchrank" type="checkbox" v-model="sBoxSchrankAdditional">
                            <label for="additionalSchrank">Zusatzausrüstung (erforderlich)</label><br>
                            <div class="line"></div>
                            <input id="prodSchreibtisch" type="checkbox" v-model="sBoxSchreibtisch">
                            <label for="prodSchreibtisch">Schreibtisch</label><br>
                            <input id="additionalSchreibtisch" type="checkbox" v-model="sBoxSchreibtischAdditional">
                            <label for="additionalSchreibtisch">Zusatzausrüstung (erforderlich)</label>
                        </div>
                        <div class="sizeM">
                            <h4><span class="underline">Box M</span></h4>
                            <input id="MprodSchrank" type="checkbox" v-model="mBoxSchrank">
                            <label for="MprodSchrank">Schrank</label><br>
                            <input id="MadditionalSchrank" type="checkbox" v-model="mBoxSchrankAdditional">
                            <label for="MadditionalSchrank">Zusatzausrüstung (erforderlich)</label><br>
                            <div class="line"></div>
                            <input id="MprodSchreibtisch" type="checkbox" v-model="mBoxSchreibtisch">
                            <label for="MprodSchreibtisch">Schreibtisch</label><br>
                            <input id="MadditionalSchreibtisch" type="checkbox" v-model="mBoxSchreibtischAdditional">
                            <label for="MadditionalSchreibtisch">Zusatzausrüstung (erforderlich)</label>
                        </div>
                        <div class="sizeL">
                            <h4><span class="underline">Box L</span> (else-Bedingung)</h4>
                            <input id="LprodSchrank" type="checkbox" disabled v-model="lBoxSchrank">
                            <label for="LprodSchrank">Schrank</label><br>
                            <input id="LadditionalSchrank" type="checkbox" disabled v-model="lBoxSchrankAdditional">
                            <label for="LadditionalSchrank">Zusatzausrüstung (erforderlich)</label><br>
                            <div class="line"></div>
                            <input id="LprodSchreibtisch" type="checkbox" disabled v-model="lBoxSchreibtisch">
                            <label for="LprodSchreibtisch">Schreibtisch</label><br>
                            <input id="LadditionalSchreibtisch" type="checkbox" disabled
                                v-model="lBoxSchreibtischAdditional">
                            <label for="LadditionalSchreibtisch">Zusatzausrüstung (erforderlich)</label>
                        </div>
                    </div>
                </div>
                <button @click="createSimulation" type="submit">⬇️ Simulation erstellen</button>
            </form>
        </template>
    </the-card>
</template>

<script>
export default {
    data() {
        return {
            name: "",
            product: "",
            tableStyle: "",
            doorType: "",
            mirrorShape: "",
            maxHeight: 20,
            maxWeight: 50,
            express: false,
            additionalEquipment: false,
            sBoxSchrank: false,
            sBoxSchrankAdditional: false,
            sBoxSchreibtisch: false,
            sBoxSchreibtischAdditional: false,
            mBoxSchrank: false,
            mBoxSchrankAdditional: false,
            mBoxSchreibtisch: false,
            mBoxSchreibtischAdditional: false
        }
    },
    computed: {
        smallBox() {
            return {
                width: this.maxHeight + "%"
            }
        },
        bigBox() {
            return {
                width: 100 - this.maxHeight + "%"
            }
        },
        wsmallBox() {
            return {
                width: this.maxWeight + "%"
            }
        },
        wbigBox() {
            return {
                width: 100 - this.maxWeight + "%"
            }
        },
        lBoxSchrank() {
            if (!this.sBoxSchrank && !this.mBoxSchrank) return true;
            if (!this.sBoxSchrankAdditional && !this.mBoxSchrankAdditional) return true;
            return false;
        },
        lBoxSchrankAdditional() {
            if (!this.sBoxSchrankAdditional && !this.mBoxSchrankAdditional && (this.sBoxSchrank || this.mBoxSchrank)) return true;
            return false;
        },
        lBoxSchreibtisch() {
            if (!this.sBoxSchreibtisch && !this.mBoxSchreibtisch) return true;
            if (!this.sBoxSchreibtischAdditional && !this.mBoxSchreibtischAdditional) return true;
            return false;
        },
        lBoxSchreibtischAdditional() {
            if (!this.sBoxSchreibtischAdditional && !this.mBoxSchreibtischAdditional && (this.sBoxSchreibtisch || this.mBoxSchreibtisch)) return true;
            return false;
        }
    },
    watch: {
        sBoxSchrank(val) {
            if (val && !this.mBoxSchrankAdditional && !this.sBoxSchrankAdditional) {
                this.mBoxSchrank = false;
            } else if (!val) {
                this.sBoxSchrankAdditional = false;
            }
        },
        sBoxSchrankAdditional(val) {
            if (val) {
                this.sBoxSchrank = true;
                this.mBoxSchrankAdditional = false;
            } else if (this.mBoxSchrank && this.sBoxSchrank && !this.mBoxSchrankAdditional && !this.sBoxSchrankAdditional) {
                this.mBoxSchrank = false;
            }
        },
        mBoxSchrank(val) {
            if (val && !this.sBoxSchrankAdditional && !this.mBoxSchrankAdditional) {
                this.sBoxSchrank = false;
            } else if (!val) {
                this.mBoxSchrankAdditional = false;
            }
        },
        mBoxSchrankAdditional(val) {
            if (val) {
                this.mBoxSchrank = true;
                this.sBoxSchrankAdditional = false;
            } else if (this.mBoxSchrank && this.sBoxSchrank && !this.mBoxSchrankAdditional && !this.sBoxSchrankAdditional) {
                this.sBoxSchrank = false;
            }
        },
        sBoxSchreibtisch(val) {
            if (val && !this.mBoxSchreibtischAdditional && !this.sBoxSchreibtischAdditional) {
                this.mBoxSchreibtisch = false;
            } else if (!val) {
                this.sBoxSchreibtischAdditional = false;
            }
        },
        sBoxSchreibtischAdditional(val) {
            if (val) {
                this.sBoxSchreibtisch = true;
                this.mBoxSchreibtischAdditional = false;
            } else if (this.mBoxSchreibtisch && this.sBoxSchreibtisch && !this.mBoxSchreibtischAdditional && !this.sBoxSchreibtischAdditional) {
                this.mBoxSchrank = false;
            }
        },
        mBoxSchreibtisch(val) {
            if (val && !this.sBoxSchreibtischAdditional && !this.mBoxSchreibtischAdditional) {
                this.sBoxSchreibtisch = false;
            } else if (!val) {
                this.mBoxSchreibtischAdditional = false;
            }
        },
        mBoxSchreibtischAdditional(val) {
            if (val) {
                this.mBoxSchreibtisch = true;
                this.sBoxSchreibtischAdditional = false;
            } else if (this.mBoxSchreibtisch && this.sBoxSchreibtisch && !this.mBoxSchreibtischAdditional && !this.sBoxSchreibtischAdditional) {
                this.sBoxSchrank = false;
            }
        },
        maxHeight(val) {
            this.maxHeight = parseInt(val);
        },

        maxWeight(val) {
            this.maxWeight = parseInt(val);
        }
    },
    methods: {
        async createSimulation() {
            if (this.name === "" || this.product === "" || (!this.sBoxSchrank && !this.sBoxSchreibtisch && !this.mBoxSchreibtisch && !this.mBoxSchrank) || (this.product === "schreibtisch" && this.tableStyle === "") || (this.product === "schrank" && this.doorType === "" && this.mirrorShape === "")) {
                return alert("Formular nicht vollständig ausgefüllt.")
            }
            if (/[^\w]/.test(this.name)) return alert("Keine Leerzeichen sowie Extrazeichen sind erlaubt (Ausnahme: _ und Zahlen)");
            let conditionS = "";
            let conditionM = "";
            const conditionSmall = "(data.gewicht &lt; data.maxGewicht) and (data.hoehe &lt; data.maxHoehe) and !data.express";
            const conditionBig = "((data.gewicht &gt;= data.maxGewicht) or (data.hoehe &gt;= data.maxHoehe)) and !data.express";
            if (this.sBoxSchrank) conditionS = "data.produkt == &quot;schrank&quot;";
            if (this.sBoxSchrank && this.sBoxSchrankAdditional) {
                conditionS += " and data.zusatzmaterial";
            } else if (this.sBoxSchrank) {
                conditionS += " and !data.zusatzmaterial";
            }
            if (this.sBoxSchrank && this.sBoxSchreibtisch) {
                conditionS = "(" + conditionS + ") or (";
            }
            if (this.sBoxSchreibtisch) conditionS += "data.produkt == &quot;schreibtisch&quot;";
            if (this.sBoxSchreibtisch && this.sBoxSchreibtischAdditional) {
                conditionS += " and data.zusatzmaterial";
            } else if (this.sBoxSchreibtisch) {
                conditionS += " and !data.zusatzmaterial";
            }
            if (this.sBoxSchrank && this.sBoxSchreibtisch) {
                conditionS = conditionS + ")";
            }
            if (this.mBoxSchrank) conditionM = "data.produkt == &quot;schrank&quot;";
            if (this.mBoxSchrank && this.mBoxSchrankAdditional) {
                conditionM += " and data.zusatzmaterial";
            } else if (this.mBoxSchrank) {
                conditionM += " and !data.zusatzmaterial";
            }
            if (this.mBoxSchrank && this.mBoxSchreibtisch) {
                conditionM = "(" + conditionM + ") or (";
            }
            if (this.mBoxSchreibtisch) conditionM += "data.produkt == &quot;schreibtisch&quot;";
            if (this.mBoxSchreibtisch && this.mBoxSchreibtischAdditional) {
                conditionM += " and data.zusatzmaterial";
            } else if (this.mBoxSchreibtisch) {
                conditionM += " and !data.zusatzmaterial";
            }
            if (this.mBoxSchrank && this.mBoxSchreibtisch) {
                conditionM = conditionM + ")";
            }
            if (conditionS === "") conditionM = "false";
            if (conditionM === "") conditionM = "false";
            let simulation = {
                conditionM: conditionM,
                conditionS: conditionS,
                conditionSmall: conditionSmall,
                conditionBig: conditionBig,
                express: this.express,
                additionalEquipment: this.additionalEquipment,
                maxHeight: this.maxHeight * 10,
                maxWeight: this.maxWeight * 10,
                name: this.name,
                product: this.product
            };
            if (this.product === "schreibtisch") {
                simulation.tableStyle = this.tableStyle;
            } else {
                simulation.doorType = this.doorType;
                simulation.mirrorShape = this.mirrorShape
            }
            await fetch('http://localhost:9033/wfms/createFile', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(simulation)
            });
            alert("Simulation wurde erstellt.")
        }
    }
}
</script>

<style scoped>
h4 {
    text-align: center;
}

.sizes {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
}

.sizes>div {
    padding: .5rem;
}

.line {
    width: 100%;
    height: 1px;
    background: #000;
    margin: .5rem 0;
}

.sizes div:nth-child(2) {
    border-left: 1px solid #000;
    border-right: 1px solid #000;
}

input[type="checkbox"] {
    width: auto;
    margin-right: .2rem;
}

.sizes label {
    display: inline;
    font-weight: normal;
}

.border {
    text-align: center;
}

.underline {
    text-decoration: underline 2px dotted #000;
    text-underline-offset: 2px;
}

.boxes {
    display: flex;
    text-align: center;
    padding-top: 1.5rem;
}

.bubble {
    top: 100%;
}

.range-slider {
    position: relative;
    height: 70px;
    width: 100%;
    margin-bottom: 2rem;
}

input[type="range"] {
    position: absolute;
    bottom: 0;
    top: 0;
    background-color: transparent;
    pointer-events: none;
    -webkit-appearance: none;
    appearance: none;
    -moz-appearance: none;
    width: 100%;
    outline: none;
    border: none;
}

input[type="range"]::-webkit-slider-runnable-track {
    -webkit-appearance: none;
    height: 5px;
}

input[type="range"]::-moz-range-track {
    -moz-appearance: none;
    height: 5px;
}

input[type="range"]::-ms-track {
    appearance: none;
    height: 5px;
}

.slider-track {
    width: 100%;
    height: 5px;
    background-color: #333;
    position: absolute;
    margin: auto;
    top: 0;
    bottom: 0;
}

input[type="range"]::-webkit-slider-thumb {
    -webkit-appearance: none;
    height: 1.1em;
    width: 1.1em;
    background-color: #3264fe;
    cursor: pointer;
    border-radius: 10px;
    pointer-events: auto;
}

input[type="range"]::-moz-range-thumb {
    appearance: none;
    -webkit-appearance: none;
    height: 1.1em;
    width: 1.1em;
    background-color: #3264fe;
    cursor: pointer;
    border-radius: 50%;
    pointer-events: auto;
}

input[type="range"]::-ms-thumb {
    appearance: none;
    height: 1.1em;
    width: 1.1em;
    background-color: #3264fe;
    cursor: pointer;
    border-radius: 50%;
    pointer-events: auto;
}

input[type="range"]:active::-webkit-slider-thumb {
    pointer-events: auto;
}

.smaller {
    font-size: .7rem;
    color: #333;
}

form {
    border: 1px solid #000;
    border-radius: 5px;
    padding: 1.5rem 4rem;
    width: 80%;
    margin: 0 auto;
    display: flex;
    flex-direction: column;
    align-items: center;
}

p,
input,
select {
    margin-bottom: 1rem;
}

label {
    display: block;
    font-weight: bold;
    user-select: none;
    margin-bottom: .2rem;
}

.inlineLabel {
    display: inline;
}

input,
select {
    padding: .2rem .4rem;
    width: 100%;
    font-weight: 400;
}

.schrank,
.schreibtisch,
.form-group {
    width: 100%;
}

.form-group.last {
    margin-top: 3rem;
}

button {
    font-size: 1.5rem;
    font-weight: bold;
    cursor: pointer;
    user-select: none;
    padding: .1rem;
    margin-top: 2rem;
}

input,
select {
    font-size: 1.2rem;
}

@media only screen and (max-width: 1200px) {
    form {
        width: 100%;
    }
}
</style>