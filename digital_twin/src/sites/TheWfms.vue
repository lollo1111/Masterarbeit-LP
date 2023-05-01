<template>
    <the-card>
        <template #header>Workflow Management System</template>
        <template #default>
            <select v-model="selectedValue">
                <option value="" disabled>Select an option</option>
                <option v-for="option in options" :key="option" :value="option">
                    {{ option }}
                </option>
            </select>
            <button @click="readFile">Read</button>
            <div @click="createInstance" class="create">
                <a>🆕<span class="textspan">Instanz erstellen</span></a>
                <p>
                    Lorem, ipsum dolor sit amet consectetur adipisicing elit. Repellendus autem provident ipsam et inventore
                    alias necessitatibus beatae perferendis, ratione placeat.
                </p>
            </div>
            <ul>
                <keep-alive>
                    <li v-for="instance in instances" :key="instance.id">
                        <one-instance :xml="instance.xml" :mode="instance.mode" :instanceId="instance.id" @delete-instance="deleteInstance"></one-instance>
                    </li>
                </keep-alive>
            </ul>
        </template>
    </the-card>
</template>

<script>
import OneInstance from '../components/OneInstance.vue';

export default {
    async created() {
        const response = await fetch("http://localhost:9033/start/files");
        this.options = await response.json();
    },
    components: {
        OneInstance
    },
    data() {
        return {
            instances: [],
            currentCounter: 0,
            selectedValue: '',
            options: [],
            xml: null
        }
    },
    methods: {
        createInstance() {
            if (!this.xml) return alert("Select a XML!");
            const id = ++this.currentCounter;
            this.instances.push({
                id: id,
                xml: this.xml,
                mode: "wait_running"
            })
        },
        deleteInstance(instance) {
            this.instances = this.instances.filter(oneInstance => oneInstance.id !== instance);
        },
        async readFile() {
            const selectedFile = this.selectedValue.split(".")[0];
            const response = await fetch(('http://localhost:9033/start/selectFile/' + selectedFile));
            this.xml = await response.text();
        }
    }
}
</script>

<style scoped>
.create {
    border: 1px solid #000;
    border-radius: 5px;
    padding: 1.5rem;
    margin-bottom: 2rem;
    cursor: pointer;
    transition: background .3s;
}

.create:hover {
    border: 2px solid #000;
    background-color: #ccc;
}

a {
    font-weight: bold;
}

ul {
    list-style: none;
}

li {
    padding: 1rem 1rem 1rem 0;
    border-bottom: 1px dotted #333;
}

li:first-child {
    border-top: 1px dotted #333;
}

.create:hover .textspan {
    text-decoration: underline 1px solid #000;
}
</style>