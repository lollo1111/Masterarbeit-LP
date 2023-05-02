<template>
    <the-card>
        <template #header>Workflow Management System</template>
        <template #default>
            <div class="btns">
                <select @change="readFile" v-model="selectedValue">
                    <option value="" disabled>Wähle eine Simulation aus</option>
                    <option v-for="option in options" :key="option" :value="option">
                        {{ option.split(".")[0] }}
                    </option>
                </select>
                <button :disabled="!xml" @click="downloadXML">💾</button>
                <button :disabled="!xml" @click="deleteXML">🗑️</button>
            </div>
            <div class="control">
                <div :class="{ hasXml: xml }" :style="clickableDiv" @click="createInstance" class="create">
                    <a>🗒️<span class="textspan">Instanz aus Vorlage erstellen</span></a>
                    <p>
                        Lorem, ipsum dolor sit amet consectetur adipisicing elit. Repellendus autem provident ipsam et
                        inventore
                        alias necessitatibus beatae perferendis, ratione placeat.
                    </p>
                </div>
                <div @click="createEmptyInstance" class="createEmpty">
                    <a>🆕<span class="textspan">Leere Instanz erstellen</span></a>
                    <p>
                        Lorem, ipsum dolor sit amet consectetur adipisicing elit. Repellendus autem provident ipsam et
                        inventore
                        alias necessitatibus beatae perferendis, ratione placeat.
                    </p>
                </div>
            </div>
            <ul>
                <keep-alive>
                    <li v-for="instance in instances" :key="instance.id">
                        <one-instance :exists="instance.exists" :xml="instance.xml" :mode="instance.mode" :instanceId="instance.id"
                            @delete-instance="deleteInstance"></one-instance>
                    </li>
                </keep-alive>
            </ul>
        </template>
    </the-card>
</template>

<script>
import OneInstance from '../components/OneInstance.vue';
import { useInstanceStore } from '../stores/InstanceStore';
const store = useInstanceStore();
export default {
    beforeRouteEnter(to, from, next) {
        next(vm => {
            async function getInstances() {
                const instances = store.getInstances;
                let highestId = 0;
                for (let instance of instances) {
                    if (instance.id > highestId) highestId = instance.id;
                    vm.instances.push({
                        id: instance.id,
                        exists: true,
                        xml: null,
                        mode: null
                    });
                }
                vm.currentCounter = highestId;
                const response = await fetch("http://localhost:9033/start/files");
                vm.options = await response.json();
            }
            getInstances();
        });
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
    computed: {
        clickableDiv() {
            if (!this.xml) {
                return {
                    cursor: 'not-allowed',
                    color: '#ccc'
                };
            } else {
                return {};
            }
        }
    },
    methods: {
        createInstance() {
            if (this.xml) {
                const id = ++this.currentCounter;
                const instance = {
                    id: id,
                    exists: false,
                    xml: this.xml,
                    mode: "wait_running"
                };
                this.instances.push(instance);
                store.addInstance(instance);
            }
        },
        createEmptyInstance() {
            const id = ++this.currentCounter;
            const instance = {
                id: id,
                exists: false,
                xml: null,
                mode: "wait_ready"
            };
            this.instances.push(instance);
            store.addInstance(instance);
        },
        async deleteXML() {
            const selectedFile = this.selectedValue.split(".")[0];
            const response = await fetch(('http://localhost:9033/start/deleteFile/' + selectedFile), {
                method: 'DELETE'
            });
            if (response.ok) {
                this.options = this.options.filter(option => option !== this.selectedValue);
                this.selectedValue = '';
                this.xml = null;
            }
        },
        async downloadXML() {
            const selectedFile = this.selectedValue.split(".")[0];
            const response = await fetch(('http://localhost:9033/start/download/' + selectedFile));
            if (!response.ok) {
                alert("Download Error")
            } else {
                const blob = await response.blob();
                const url = window.URL.createObjectURL(blob);
                const link = document.createElement('a');
                link.href = url;
                link.setAttribute('download', (selectedFile + '.xml'));
                link.style.display = 'none';
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
                window.URL.revokeObjectURL(url);
            }
        },
        deleteInstance(instance) {
            this.instances = this.instances.filter(oneInstance => oneInstance.id !== instance);
            store.deleteInstance(instance);
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
.btns button {
    margin-left: .5rem;
    padding: .2rem;
    font-size: 1.2rem;
    cursor: pointer;
}

select {
    font-size: 1.2rem;
    padding: .34rem;
}

.btns {
    margin-bottom: 1rem;
    display: flex;
    align-items: center;
}

.control {
    display: flex;
    gap: 0 2rem;
}

.create,
.createEmpty {
    outline: 1px solid #000;
    border-radius: 5px;
    padding: 1.5rem;
    margin-bottom: 2rem;
    cursor: pointer;
    transition: background .3s;
}

.createEmpty:hover,
.hasXml:hover {
    outline-width: 2px;
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

.createEmpty:hover .textspan,
.hasXml:hover .textspan {
    text-decoration: underline 1px solid #000;
}
</style>