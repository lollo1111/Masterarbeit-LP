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
                <p>
                    Lorem ipsum, dolor sit amet consectetur adipisicing elit. Nulla officia quis rerum dolorum consequuntur
                    fugiat eius, odio error quo sint ratione voluptates dolorem atque et nam similique esse dicta
                    accusantium temporibus veniam ipsum alias provident ex. Laboriosam, harum iusto facilis ab dignissimos
                    inventore deleniti amet! Autem, obcaecati et. Incidunt necessitatibus totam itaque quia corrupti id
                    temporibus nisi quam alias dicta enim ipsum fugiat commodi quidem perspiciatis rerum ullam maxime
                    sapiente aperiam unde, repellat aspernatur cupiditate excepturi. Iure, magnam dolor nemo nostrum alias
                    exercitationem optio facilis sunt eaque minima culpa corrupti, odit quisquam, consequuntur nobis ducimus
                    reiciendis saepe? Dolore dignissimos mollitia quidem, iusto distinctio magnam tempora explicabo neque
                    rerum, maxime repellat sit voluptatem veniam quod odio nihil. Obcaecati, culpa perferendis distinctio
                    ipsum odio voluptatibus perspiciatis alias neque iste natus sapiente vel dicta aperiam deserunt
                    accusantium nulla minus praesentium necessitatibus. Provident esse enim, aliquam laboriosam numquam fuga
                    voluptatibus, vel voluptate pariatur magni odio neque earum nulla molestias! Labore quasi accusamus
                    vero! Sapiente et modi minima voluptas iusto alias, soluta vero dolorum fugiat consectetur doloribus
                    aliquid incidunt suscipit ipsam qui asperiores impedit nam enim, unde atque, exercitationem sed quasi.
                    Nemo tempora corporis voluptatum reprehenderit eos quas. Nesciunt molestias fuga sit, quas voluptate
                    odio atque recusandae placeat sint voluptatem impedit numquam voluptates quo pariatur perferendis ullam
                    dolorum est, beatae dolor nobis incidunt. Dolores hic rem at illum vero atque soluta quae. Ullam magnam,
                    doloribus expedita ut quod nam sint est a, laborum, aut cupiditate labore tenetur aliquid veritatis
                    voluptatibus accusantium consequatur consequuntur inventore repellendus culpa iure omnis minima!
                    Voluptatem iusto animi eaque quia exercitationem ut recusandae quam excepturi qui, pariatur id
                    temporibus at cum, in aut quisquam consequuntur eos facere accusantium sunt totam distinctio adipisci.
                    Rerum numquam rem doloribus sunt ea sapiente cupiditate consectetur animi architecto et atque culpa,
                    inventore eaque iure nihil sed eum corrupti laudantium maiores aspernatur provident sint iusto omnis?
                    Libero quisquam nam aliquam quia natus quasi aspernatur voluptate unde, quaerat dicta asperiores iure
                    maxime impedit doloremque fugiat sequi nisi blanditiis at praesentium vero! Odio libero dolore facere.
                    Necessitatibus at, neque harum quibusdam deleniti fuga et consequuntur amet quaerat quisquam laudantium
                    tempora hic dolorum saepe culpa molestiae nesciunt repudiandae vitae sunt. Sed ullam eligendi beatae
                    aspernatur ad natus, tempora neque corporis dignissimos maiores! Harum dolorum ratione doloribus impedit
                    aliquid eos ipsum eius, sunt, corporis magnam neque deleniti quod, omnis porro aperiam repellendus rem
                    commodi quaerat eum. Maxime, nostrum ullam nisi sequi vitae natus molestias laboriosam rem expedita
                    quidem reiciendis exercitationem. Praesentium nesciunt dignissimos itaque earum ipsum deserunt numquam.
                    Sed architecto quod inventore dolore nulla adipisci beatae dolorum similique consequatur earum debitis
                    eveniet minus rem obcaecati blanditiis modi repellat eaque, dolores culpa ullam odio! Architecto id
                    laboriosam saepe rem ex, mollitia suscipit nulla placeat ratione dolore. Numquam eligendi tempora sequi
                    dignissimos neque expedita possimus dolore perferendis vitae fugit minus iste quasi magni, nulla
                    corrupti porro id nemo accusamus. Officia facilis cumque maxime est quibusdam maiores autem, voluptatem
                    error corrupti reprehenderit perspiciatis quis recusandae natus quidem aperiam eius quisquam? Minus,
                    temporibus esse ab fugiat reprehenderit accusamus dolor facilis distinctio veritatis, aliquid corporis
                    delectus. Velit quo eius ipsum, officiis voluptatum delectus, deserunt quasi doloribus id possimus
                    cupiditate dolore! Modi adipisci, numquam maiores vitae, at aut quaerat vel nihil exercitationem quos
                    eveniet animi, ipsam natus magni incidunt explicabo accusantium! Iste nisi ipsum cum corrupti impedit
                    veniam error sed corporis veritatis temporibus quam magnam minima, nemo cumque quasi delectus porro?
                    Eveniet ducimus eius obcaecati! Assumenda, quos eligendi deserunt voluptates atque ducimus eius corporis
                    quaerat odio? Neque laudantium et velit unde optio repellat similique hic error. Mollitia cum minima,
                    atque cupiditate iste magnam ut culpa ab magni. Tempore quasi obcaecati blanditiis totam et quibusdam
                    laboriosam incidunt libero nobis non optio hic, a quod commodi ducimus consequuntur ipsam distinctio,
                    nulla natus vero velit. Recusandae qui, nobis saepe blanditiis cum, iusto labore voluptatem dolor
                    ducimus odit quod. Nisi saepe aspernatur sit, minima magni explicabo exercitationem eaque qui aliquid id
                    repudiandae pariatur obcaecati atque quidem in quam laudantium et soluta distinctio error veritatis.
                    Fugiat illum omnis cupiditate, architecto debitis deserunt cum nihil consequuntur est ipsa ut quos,
                    assumenda ad sint nisi modi temporibus molestias alias fuga quaerat voluptas non necessitatibus nobis?
                    Ut temporibus dolore, quod saepe veniam possimus officia esse dolor asperiores laborum minus. Cum, ipsum
                    consectetur. Sunt alias necessitatibus quaerat distinctio libero blanditiis sapiente officiis unde, non,
                    laudantium, corporis odit ratione facilis explicabo voluptates ex? Maiores, perferendis? Iure sint eaque
                    tenetur placeat, repellendus cum sapiente qui eius doloribus incidunt necessitatibus asperiores optio
                    illo, non, voluptatibus sed commodi! Obcaecati magnam sequi dolorem natus velit, porro at delectus
                    explicabo similique temporibus error ad omnis commodi. Qui tenetur obcaecati consectetur quis nobis
                    officiis, esse minus ipsa officia. Rerum ad qui explicabo ducimus dignissimos amet laborum obcaecati
                    beatae harum, deleniti voluptas exercitationem ut corporis dolor consequatur debitis maiores veritatis
                    officiis atque sequi laudantium soluta ea magni id? Fugiat eligendi minima delectus deserunt odit
                    architecto enim aliquid laudantium quidem possimus doloremque nihil, tenetur tempore dolorum nostrum
                    consectetur alias repellendus! Nam blanditiis recusandae quaerat earum tempora voluptate quia porro
                    alias dolorem! Quaerat eos vitae praesentium exercitationem, quas labore sapiente distinctio facere
                    error maiores repellat! Natus ex nisi quibusdam atque obcaecati veritatis molestiae sunt iusto qui
                    quisquam, doloribus excepturi corrupti dolores ullam! Soluta eum, ad nobis animi deserunt distinctio
                    veritatis error quis quia, ducimus atque praesentium illo eveniet vitae dolore rem vel pariatur iusto
                    repellendus hic. Itaque consectetur illum cumque debitis officia et at dicta laborum pariatur
                    consequuntur. Exercitationem itaque eaque esse ad repellendus soluta quo molestias ullam dignissimos
                    odit commodi, sunt facere dolore enim quod id necessitatibus ipsum voluptatum nulla? Laborum cum omnis
                    nesciunt et nobis dolore, obcaecati, dolorum voluptas rerum facilis eveniet amet provident eaque vel
                    pariatur aut. Commodi quaerat distinctio earum! Repudiandae similique aliquam voluptate officiis
                    dignissimos? Magnam, placeat officiis possimus voluptates inventore reiciendis pariatur enim molestias
                    explicabo sunt tempora consequuntur recusandae quo ab laboriosam doloremque animi ratione dignissimos
                    ipsa temporibus! Iste explicabo rerum quo a odit fugit architecto aspernatur porro in repellat impedit,
                    magnam ipsum culpa id. Cupiditate quisquam sapiente facere dolor?
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