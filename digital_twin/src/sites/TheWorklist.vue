<template>
    <the-card>
        <template #header>Worklist</template>
        <template #default>
            <div class="container">
                <dialog :open="filter">
                    <the-card>
                        <div class="all">
                            <div class="filter">
                                <h1>Filter</h1>
                                <button @click="toggleFilter">Bestätigen</button>
                            </div>
                        </div>
                    </the-card>
                </dialog>
                <a @click="refresh">🔄️<span class="spantext">Aktualisieren</span></a> | <a @click="toggleFilter">🎚️<span
                        class="spantext">Filter</span></a> | <a @click="toggleFilter">⏩<span class="spantext">Automatisch
                        aktualisieren</span></a>
                <p>
                    Lorem, ipsum dolor sit amet consectetur adipisicing elit. Repellendus autem provident ipsam et inventore
                    alias necessitatibus beatae perferendis, ratione placeat.
                </p>
                <ul>
                    <li :class="{ activeSimulation: item.items.length > 0 }" v-for="item of worklist" :key="item.task">
                        <device-group :item="item"></device-group>
                    </li>
                </ul>
            </div>
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
            scrollPosition: 0
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
            if (this.filter) {
                const scrollPosition = window.pageYOffset || document.documentElement.scrollTop;
                this.scrollPosition = scrollPosition;
                document.body.classList.add('modal-open');
                document.body.style.top = `-${scrollPosition}px`;
            } else {
                // Remove the 'modal-open' class from the body
                document.body.classList.remove('modal-open');
                // Reset the scroll position explicitly on the body
                // Scroll the page back to the original position
                window.scrollTo(0, this.scrollPosition);
            }
        }
    }
}
</script>

<style scoped>
.container {
    position: relative;
}

.all {
    height: 500px;
    width: 500px;
    margin: 0 auto;
    border: 1px solid #000;
    border-radius: 5px;
    background: #fff;
}

dialog {
    position: fixed;
    top: 0;
    left: 50%;
    transform: translateX(-50%);
    width: 100%;
    height: 100%;
    z-index: 10;
    background: transparent;
    border: none;
}

dialog::after {
    content: '';
    background: rgba(255, 255, 255, 0.5);
    backdrop-filter: blur(5px);
    position: fixed;
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

.filter {
    width: 100%;
    height: 100%;
    padding: 2rem;
    text-align: center;
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