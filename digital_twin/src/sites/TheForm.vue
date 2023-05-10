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
            mirrorShape: ""
        }
    }
}
</script>

<style scoped>
.smaller {
    font-size: .7rem;
    color: #333;
}

form {
    border: 1px solid #000;
    border-radius: 5px;
    padding: 1.5rem;
    width: 50%;
    margin: 0 auto;
    display: flex;
    flex-direction: column;
    align-items: center;
}

p,
input {
    margin-bottom: 1rem;
}

label {
    display: block;
    font-weight: bold;
    user-select: none;
}

input {
    padding: .2rem .4rem;
    width: 100%;
    font-weight: 400;
}

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