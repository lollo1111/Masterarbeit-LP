<template>
    <the-card>
        <template #header>Worklist</template>
        <template #default>
            <a @click="refresh">🔄️<span class="spantext">Aktualisieren</span></a> | <a @click="toggleFilter">🎚️<span class="spantext">Filter</span></a> | <a @click="toggleFilter">⏩<span class="spantext">Automatisch aktualisieren</span></a>
            <dialog :open="filter">
                <p>Greetings, one and all!</p>
                <button @click="filter = false">OK</button>
            </dialog>
            <p>
                Lorem, ipsum dolor sit amet consectetur adipisicing elit. Repellendus autem provident ipsam et inventore
                alias necessitatibus beatae perferendis, ratione placeat.
            </p>
            <ul>
                <li :class="{activeSimulation: item.items.length > 0}" v-for="item of worklist" :key="item.task">
                    <device-group :item="item"></device-group>
                </li>
            </ul>
        </template>
    </the-card>
</template>

<script>
import DeviceGroup from '../components/DeviceGroup.vue';
import { useInstanceStore } from '../stores/InstanceStore';
const store = useInstanceStore();
export default {
    async created() {
        await this.refresh();

    },
    components: {
        DeviceGroup
    },
    data() {
        return {
            filter: false,
        }
    },
    computed: {
        worklist() {
            return store.getWorklist;
        }
    },
    methods: {
        async refresh() {
            await store.loadWorklist();
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
    grid-template-columns: repeat(1, 1fr);
    grid-template-rows: min-content;
    gap: 1rem;
}

ul li {
    border-radius: 5px;
    border: 1px solid #000;
    padding: .5rem;
}

.activeSimulation {
    animation: borderAnimation infinite 5s;
    border-width: 10px;
}

a {
    cursor: pointer;
}

a:hover .spantext {
    text-decoration: underline solid #000;
}
@keyframes borderAnimation {
    0% {
        border-color: red;
    }
    20% {
        border-color: blue;
    }
    60% {
        border-color: gold;
    }
    80% {
        border-color: red;
    }
    100% {
        border-color: red;
    }
}
</style>