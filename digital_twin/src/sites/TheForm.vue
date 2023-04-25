<template>
    <the-card>
        <template #header>Simulation definieren</template>
        <template #default>
            <p>
                Lorem, ipsum dolor sit amet consectetur adipisicing elit. Repellendus autem provident ipsam et inventore alias necessitatibus beatae perferendis, ratione placeat.
            </p>
            <form @submit.prevent>
                <div class="form-group">
                    <label for="name">Simulation Name</label>
                    <input id="name" type="text">
                </div>
                <div class="form-group">
                    <label for="name">Simulation Name</label>
                    <input id="name" type="text">
                </div>
                <div class="form-group">
                    <label for="name">Simulation Name</label>
                    <input id="name" type="text">
                </div>
                <div class="form-group">
                    <label for="name">Simulation Name</label>
                    <input id="name" type="text">
                </div>
                <button type="submit">⬇️ Simulation erstellen</button>
            </form>
        </template>
    </the-card>
</template>

<style scoped>
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
}
</style>