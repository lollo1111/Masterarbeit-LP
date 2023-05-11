<template>
    <the-card>
        <template #header>Simulation definieren</template>
        <template #default>
            <p>
                Lorem, ipsum dolor sit amet consectetur adipisicing elit. Repellendus autem provident ipsam et inventore
                alias necessitatibus beatae perferendis, ratione placeat.
            </p>
            <form @submit.prevent>
                <div class="form-group">
                    <label for="name">Simulation Name <span class="smaller">(überschreibt bereits existierende Simulationen
                            mit demselben Namen)</span></label>
                    <input id="name" type="text">
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
                        <label for="style">Wähle einen Stil aus</label>
                        <select id="style" v-model="style">
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
                    <label for="min">Paket abhängig von <span class="underline">Höhe</span> für Versand kategorisieren:</label>
                    <div class="range-slider">
                        <div class="slider-track"></div>
                        <div class="border">Die Grenze liegt bei >= {{ minHeight }} cm</div>
                        <input class="lower" type="range" id="min" name="min" min="0" max="100" unit="📡" v-model="minHeight">
                        <div class="boxes">
                            <div class="bubble" :style="smallBox">Kleines Paket</div>
                            <div class="bubble" :style="bigBox">Großes Paket</div>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <label for="min">Paket abhängig von <span class="underline">Gewicht</span> für Versand kategorisieren:</label>
                    <div class="range-slider">
                        <div class="slider-track"></div>
                        <div class="border">Die Grenze liegt bei >= {{ minWeight }} g</div>
                        <input class="lower" type="range" id="wmin" name="min" min="0" max="100" v-model="minWeight">
                        <div class="boxes">
                            <div class="bubble" :style="wsmallBox">Kleines Paket</div>
                            <div class="bubble" :style="wbigBox">Großes Paket</div>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <label for="min">Paketgrößen festlegen:</label>
                    <div class="sizes">
                        <div class="sizeS">
                            <h4>Box S</h4>
                            <input id="prodSchrank" type="checkbox">
                            <label for="prodSchrank">Schrank</label><br>
                            <input id="additionalSchrank" type="checkbox">
                            <label for="additionalSchrank">Zusatzausrüstung (erforderlich)</label><br>
                            <div class="line"></div>
                            <input id="prodSchreibtisch" type="checkbox">
                            <label for="prodSchreibtisch">Schreibtisch</label><br>
                            <input id="additionalSchreibtisch" type="checkbox">
                            <label for="additionalSchreibtisch">Zusatzausrüstung  (erforderlich)</label>
                        </div>
                        <div class="sizeM">
                            <h4>Box M</h4>
                            <input id="MprodSchrank" type="checkbox">
                            <label for="MprodSchrank">Schrank</label><br>
                            <input id="MadditionalSchrank" type="checkbox">
                            <label for="MadditionalSchrank">Zusatzausrüstung  (erforderlich)</label><br>
                            <div class="line"></div>
                            <input id="MprodSchreibtisch" type="checkbox">
                            <label for="MprodSchreibtisch">Schreibtisch</label><br>
                            <input id="MadditionalSchreibtisch" type="checkbox">
                            <label for="MadditionalSchreibtisch">Zusatzausrüstung  (erforderlich)</label>
                        </div>
                        <div class="sizeL">
                            <h4>Box L</h4>
                            <input id="LprodSchrank" type="checkbox">
                            <label for="LprodSchrank">Schrank</label><br>
                            <input id="LadditionalSchrank" type="checkbox">
                            <label for="LadditionalSchrank">Zusatzausrüstung  (erforderlich)</label><br>
                            <div class="line"></div>
                            <input id="LprodSchreibtisch" type="checkbox">
                            <label for="LprodSchreibtisch">Schreibtisch</label><br>
                            <input id="LadditionalSchreibtisch" type="checkbox">
                            <label for="LadditionalSchreibtisch">Zusatzausrüstung  (erforderlich)</label>                          
                        </div>
                    </div>
                </div>
                <button type="submit">⬇️ Simulation erstellen</button>
            </form>
        </template>
    </the-card>
</template>

<script>
export default {
    data() {
        return {
            product: "",
            style: "",
            doorType: "",
            mirrorShape: "",
            minHeight: 50,
            minWeight: 50,
        }
    },
    computed: {
        smallBox() {
            return {
                width: this.minHeight + "%"
            }
        },
        bigBox() {
            return {
                width: 100 - this.minHeight + "%"
            }
        },
        wsmallBox() {
            return {
                width: this.minWeight + "%"
            }
        },
        wbigBox() {
            return {
                width: 100 - this.minWeight + "%"
            }
        },
    },
    watch: {
        minHeight(val) {
            this.rangge = parseInt(val);
        },

        minWeight(val) {
            this.minWeight = parseInt(val);
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

.sizes > div {
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
    height: 1.7em;
    width: 1.7em;
    background-color: #3264fe;
    cursor: pointer;
    border-radius: 10px;
    pointer-events: auto;
}

input[type="range"]::-moz-range-thumb {
    appearance: none;
    -webkit-appearance: none;
    height: 1.7em;
    width: 1.7em;
    background-color: #3264fe;
    cursor: pointer;
    border-radius: 50%;
    pointer-events: auto;
}

input[type="range"]::-ms-thumb {
    appearance: none;
    height: 1.7em;
    width: 1.7em;
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
    width: 70%;
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

button {
    font-size: inherit;
    font-weight: bold;
    cursor: pointer;
    user-select: none;
    padding: .1rem;
}

@media only screen and (max-width: 1200px) {
    form {
        width: 100%;
    }
}
</style>