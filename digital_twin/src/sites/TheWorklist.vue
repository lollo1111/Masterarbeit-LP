<template>
    <the-card>
        <template #header>Worklist</template>
        <template #default>
            <a @click="refresh">🔄️<span class="spantext">Aktualisieren</span></a>
            <a @click="toggleFilter">🎚️Filter</a>
            <dialog :open="filter">
                <p>Greetings, one and all!</p>
                <button @click="filter = false">OK</button>
            </dialog>
            <p>
                Lorem, ipsum dolor sit amet consectetur adipisicing elit. Repellendus autem provident ipsam et inventore
                alias necessitatibus beatae perferendis, ratione placeat.
            </p>
            <ul>
                <li v-for="device in devices" :key="device.id">
                    <one-device>
                        <template #header>{{ device.name }}</template>
                        <template #default>
                            <p>{{ device.type }}</p>
                            <p>{{ device.value }}</p>
                        </template>
                    </one-device>
                </li>
            </ul>
        </template>
    </the-card>
</template>

<script>
import OneDevice from '../components/OneDevice.vue';
export default {
    async created() {
        await this.refresh();

    },
    components: {
        OneDevice
    },
    data() {
        return {
            devices: null,
            filter: false
        }
    },
    methods: {
        async refresh() {
            const response = await fetch("http://localhost:7410/api/tags");
            this.devices = await response.json();
        },
        toggleFilter() {
            this.filter = !this.filter;
        }
    }
}
</script>

<style scoped>
dialog {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    z-index: 10;
}

dialog::after {
    content: '';
    background: red;
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    z-index: -1;
}
ul {
    margin-top: 1rem;
    list-style: none;
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 1rem;
}

ul li {
    border-radius: 5px;
    border: 1px solid #000;
    padding: .5rem;
}

a {
    cursor: pointer;
}

a:hover .spantext {
    text-decoration: underline solid #000;
}
</style>