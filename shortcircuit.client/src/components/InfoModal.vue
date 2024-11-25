<template>
    <q-card dark class="card">
        <q-tabs v-model="selectedTab">
            <q-tab v-for="tab in tabs" :name="tab" :label="tab" :key="tab" />
        </q-tabs>
        <q-carousel v-model="selectedTab" animated class="bg-grey-9" style="display: table-row; flex: 1; width: 100%;">
            <q-carousel-slide name="Info" style="display: flex; flex-direction: column; align-items: center">
                <q-card-section dark class="card-section" style="">
                    This application serves to allow people to decode the "save.dat" file from Splatoon 3 to inspect it's contents. Currently, I am not offering any way of editing this file.
                    <br><br>
                    To use the program, you can drag and drop (or just click to open a dialog) to upload your save.dat on the top right of the page. If successful, 3 tabs will be visible.
                    The data in this file is represented as JSON files.
                    <br>
                    <h5>Meta</h5> 
                    Contains data from the decrypted body's header. This includes the game version that saved the given file.
                    <br>
                    <h5>Client</h5>
                    Contains data that is solely owned by the client. This contains singleplayer progress, player appearance, control options, alongside other things that Nintendo does not consider important to stop from a user tampering with.
                    <br>
                    <h5>Server</h5>
                    Contains a cache of the server-owned portion of the save. This contains any information Nintendo wants to prevent users from tampering with (without a paper trail). This includes gear/weapons that are unlocked and tracking one-time rewards in singleplayer.
                </q-card-section>
                <span class="bottom-of-parent" style="text-align: center; font-size: xx-small; padding-bottom: 10px">This website is not affiliated with Nintendo Co., Ltd. All product names, logos, and brands are property of their respective owners.</span>
            </q-carousel-slide>
            <q-carousel-slide name="Contact">
                <q-card-section dark class="card-section">
                    Due to life circumstances, I am not providing support for using this application (sorry).
                    <br><br>
                    For contributions, please use the GitHub repo's pull requests.
                    <br>
                    For bug reports, please use the GitHub repo's issues page.
                    <br>
                    Please only contact me directly if you have security concerns about the hosting of the application.
                    <br><br>
                    Given that, the GitHub repo is hosted <a href="https://github.com/shadowninja108/ShortCircuit">here.</a> You can reach out to me on <span title="I'm not calling it X, fuck you.">Twitter</span> at <a href="https://x.com/shadowninja108">@shadowninja108</a>.
                </q-card-section>
            </q-carousel-slide>
            <q-carousel-slide name="Technical">
                <q-card-section dark class="card-section">
                    Splatoon 3 (codenamed Thunder) stores in two segments, client-owned and server-owned.
                    <br><br>
                    <h5>Client</h5>
                    Mainly is comprised of mission progress and various other things that need to be writable offline (such as if a weapon is "new" in the Customize scene).
                    <br><br>
                    <h5>Server</h5>
                    Contains what Nintendo decided they want to keep an eye on (such as what weapons you have unlocked). The client must request changes to the server and include the reason for the change.
                    This allows Nintendo to always be able to audit what is in a player's save and take appropriate action (such as banning).
                    <br><br>
                    The game still needs both sections on hand offline, so a cache of the server-owned save is stored locally. 
                    The local save's body is just each section encoded as messagepack (with all keys hashed) joined together with a small header.
                    This is then encrypted with the standard EPD crypto memery™.
                    <br><br>
                    Strings for keys are not compile-time hashed, so they are just plaintext in the executable. Combined with the fact no formatting is done to build said strings, 
                    it's trivial to rip them out naively with just running strings on the decompressed executable. I offer an API to request my dictionary, see the Swag(ger) docs.
                    <br><br>
                    I offer an API for decoding these saves for the backend, and it's free to use by anyone. Given I am hosting this out of pocket, all that I ask of developers is reasonable rate limiting be enforced in their clients and don't be a nuisance.
                    Swag(ger) docs are provided, see the bottom right of the page. All client/server code (excluding decoding/deserializing) is public (see Contact tab), if you want to audit that or host your own site.
                    <br><br>
                    Out of fear of it being abused, I've omitted the decryption/decoding from public view, that is handled on the backend.
                    I do intend to release this code eventually, but until then I feel it's important people still be able to decode their saves. I will update this message when this changes.
                    <br><br>
                    PS. Please do not bother me about releasing the backend code.
                </q-card-section>
            </q-carousel-slide>
        </q-carousel>
        <div class="unselectable wl" title="With love">❤️</div>
    </q-card>
</template>

<script setup lang="ts">
import { ref, Ref } from 'vue'

const tabs: Ref<string[]> = ref(['Info', 'Contact', 'Technical'])
const selectedTab: Ref<string> = ref(tabs.value[0])
</script>

<style scoped>

.card {
    min-width: 700px;
    height: 600px;
    display: flex;
    overflow-y: hidden;
    flex-direction: column;
    align-items: center;
}

.card-section {
    position: relative;
}

.wl {
    /*! ❤️ */
    color: #3a193e;
    padding-top: 10px;
    padding-bottom: 10px;
    width: fit-content;
    text-align: center;
    flex: 0;
}

.unselectable {
    -webkit-touch-callout: none;
    -webkit-user-select: none;
    -khtml-user-select: none;
    -moz-user-select: none;
    -ms-user-select: none;
    user-select: none;
}

.bottom-of-parent {
    position: absolute; 
    bottom: 0px;
}
</style>
