<template>

    <q-layout view="lHh lpr lFf" container style="display: flex; min-height: inherit" class="shadow-2 rounded-borders">
        <q-dialog v-model="dialogRef">
            <InfoModal />
        </q-dialog>

        <q-header elevated style="display: table; width: 100%">
            <div style="display: flex; justify-content: space-between">
                <div class="title">
                    <h2>ShortCircuit</h2>
                    <h3>Thunder save decoder</h3>
                </div>

                <!-- Tabs if we have a save model. -->
                <q-tabs v-if="modelRef !== null" v-model="tabRef" style="max-height: 60px; align-self: flex-end">
                    <q-tab 
                        v-for="index in range(0, modelRef.Models.length)" 
                        :key="index" 
                        :name="index"
                        :label="getTabLabel(index)" 
                    />
                </q-tabs>

                <div class="upload-component">
                    <!-- Save will never be more than 0x100000 bytes, a hardcoded maximum in SaveDataModule. -->
                    <Uploader
                        label="Click or drag/drop your save.dat here"
                        url="api/save/decode"
                        @success="onUploadSuccess"
                        :validate="validate"
                        :max-length="1048576"
                    />
                </div>
            </div>

        </q-header>
        <q-footer elevated style="display: flex; justify-content: space-between">
            <q-btn @click="dialogRef = true" no-caps align="left">
                Information
            </q-btn>
            <q-btn href="swagger" no-caps align="right">
                Swag(ger) Docs
            </q-btn>
        </q-footer>

        <q-page-container>
            <!-- When there is no save model yet. -->
            <q-card dark v-if="modelRef === null" style="width: 400px; margin: auto">
                <div style="text-align: center" class="text-h2">Upload your save file on the top right.</div>
            </q-card>


            <!-- stupid height style so that the editor actually fits the parent... i love web dev -->
            <q-carousel
                v-model="tabRef"
                transition-prev="slide-right"
                transition-next="slide-left" 
                animated
                style="height: calc(100vh - 176px)"
                @beforeTransition="onNavigate"
                v-if="modelRef !== null && tabsRef !== null"
            > 
                <q-carousel-slide
                    v-for="editor in editorsRef"
                    :name="editor.index"
                >
                    <vue-monaco-editor
                        theme="vs-dark" 
                        language="json"
                        save-view-state
                        :v-model="editor.monaco"
                        :options="OPTIONS"
                        @mount="editor.onMount($event)"
                    />
                </q-carousel-slide>
            </q-carousel>

        </q-page-container>
    </q-layout>
</template>

<script setup lang="ts">
import InfoModal from './InfoModal.vue'
import Uploader from './Uploader.vue'

import { ref, Ref, shallowRef } from 'vue'
import * as monacoEditor from 'monaco-editor/esm/vs/editor/editor.api';

enum ResponseType {
    Success = "Success",
    Cancelled = "Cancelled",
    SaveBad = "SaveBad",
    SaveTooBig = "SaveTooBig",
    UnknownFailure = "UnknownFailure",
}

interface ResponseBody<T> {
    Type: ResponseType;
    Value: T | null;
    Message: string;
}

interface SaveBodyHeader {
    Version: number;
    GameVersion: number;
}

interface SaveModel {
    BodyHeader: SaveBodyHeader;
    Models: any[];
}

class Editor {
    index: number
    text: string
    monaco: monacoEditor.editor.IStandaloneCodeEditor | null
    state: monacoEditor.editor.ICodeEditorViewState | null

    constructor(index: number) {
        this.index = index
        this.text = ""
        this.monaco = null
        this.state = null
    }

    public onMount(newMonaco: monacoEditor.editor.IStandaloneCodeEditor) {
        this.monaco = newMonaco
        this.update()
        this.onNavigateTo()
    }

    public onNavigateAway() {
        /* Store the view state. */
        const monaco = this.monaco
        if(monaco !== null) {
            this.state = monaco.saveViewState()
        }
    }

    public onNavigateTo() {
        /* If there's a view state to restore, do so, then clear it. */
        if(this.state !== null) {
            this.monaco?.restoreViewState(this.state)
            this.state = null
        }
    }

    public update() {
        this.monaco?.setValue(this.text)
    }
}

const OPTIONS = {
    automaticLayout: true,
    formatOnType: true,
    formatOnPaste: true,
    readOnly: true
};

const tabLabels: string[] = ["Meta", "Client", "Server"]

const modelRef: Ref<SaveModel | null> = ref(null)
const tabRef: Ref<number | null> = ref(null)
const tabsRef: Ref<string[] | null> = ref(null)
const editorsRef: Ref<Editor[]> = shallowRef([])

const dialogRef: Ref<boolean> = ref(false)

async function onUploadSuccess(response: Response) {
    const body: ResponseBody<SaveModel> = await response.json()

    /* Shouldn't happen... */
    if (body.Type !== ResponseType.Success) {
        return
    }

    /* Load model into editors. */
    setModel(body.Value!!)
    /* Then navigate to the meta tab. */
    tabRef.value = 0

    console.log("Loaded save for game version " + getGameVersionString(body.Value!!.BodyHeader))
}

async function validate(response: Response): Promise<string | null> {
    const body: ResponseBody<SaveModel> = await response.json()
    if(body.Type === ResponseType.Success) {
        return null
    }

    return body.Message
}

function onNavigate(newIdx: string | number, oldIdx: string | number) {
    let oldEditor: Editor | undefined = editorsRef.value[oldIdx as number]

    if(oldEditor !== undefined) {
        oldEditor.onNavigateAway()
    }
}

function* range(start: number, end: number): Generator<number> {
    for (let i = start; i <= end; i++) {
        yield i
    }
}

function getTabLabel(index: number): string {
    if (index >= tabLabels.length)
        return "Unknown " + index
    return tabLabels[index]
}

function toPrettyJson(object: any): string {
    return JSON.stringify(object, null, 2)
}

function setModel(model: SaveModel): void {
    const tabs: string[] = []

    tabs.push(toPrettyJson(model.BodyHeader))
    for (const m of model.Models) {
        tabs.push(toPrettyJson(m))
    }

    /* Resize editor array to fit everything. */
    let editors: Editor[] = editorsRef.value
    editors.length = tabs.length
    for(let i = 0; i < tabs.length; i++) {
        let editor: Editor = editors[i]
        
        /* Create editor object if it doesn't exist. */
        if(editor === null || editor === undefined) {
            editor = new Editor(i);
        }
        
        /* Set the text and update editor. */
        editor.text = tabs[i];
        editor.update();

        /* Clear view state, as we've changed the text so it's nonsense anyways. */
        /* TODO: if you upload a file while on a tab that's not meta, the state still gets written (since it's navigated away from). Returning to that tab then scrolls to a nonsense position. */
        editor.state = null;

        /* Store editor object. */
        editors[i] = editor
    }
    
    tabsRef.value = tabs
    modelRef.value = model
    editorsRef.value = editors
}

function getGameVersionString(header: SaveBodyHeader): string {
    let ret = "";
    let v = header.GameVersion;
    while (v > 0)
    {
        let d = (v & 0xff).toString();
        v = v >> 8;

        if (v > 0)
            d = "." + d;

        ret = d + ret;
    }
    return ret;
}
</script>

<script lang="ts">
import '@dafcoe/vue-file-uploader/style.css'
import { VueMonacoEditor } from '@guolao/vue-monaco-editor'

export default {
    components: {
        VueMonacoEditor
    },
};

</script>

<style scoped>
header {
    display: flex;
    justify-content: space-between;
}

.title {
    float: left;
    margin: 15px;
}

.upload-component {
    justify-items: right;
}

.monaco-editor {
    height: 100%;
}
</style>
