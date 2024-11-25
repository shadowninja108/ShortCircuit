<template>
    <Picker
        :label="labelRef"
        accept=".dat"
        :error-message="errorMessageRef"
        @selected="onSelected"
    />
</template>

<script setup lang="ts">
import { ref, Ref } from "vue";
import Picker from "./Picker.vue"

const emit = defineEmits<{
    (e: 'success', response: Response)
    (e: 'fail', message: string)
}>()

type ValidateCallback = (response: Response) => Promise<string | null>

const props = defineProps<{
    label: string,
    url: string,
    maxLength: number,
    validate: ValidateCallback | null
}>()

const labelRef: Ref<string> = ref(props.label)
const errorMessageRef: Ref<string | undefined> = ref(undefined)

function onFail(message: string) {
    emit('fail', message)
    labelRef.value = props.label
    /* Present message. */
    errorMessageRef.value = message
    setTimeout(() => {
        /* Then hide it 3 seconds later. */
        errorMessageRef.value = undefined
    }, 3000)
}

async function onSelected(file: File) {
    /* Good ol' client-side validation. */
    if(file.size > props.maxLength) {
        onFail("File size too large.")
        return
    }

    try {
        labelRef.value = "Uploading..."

        const response = await fetch(props.url, {
            method: 'POST',
            body: file,
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/octet-stream' 
            }
        })

        /* If validate function provided, try using it. */
        if(props.validate !== null) {
            const errorMessage = await props.validate(response.clone())
            if(errorMessage !== null) {
                onFail(errorMessage)
                return
            }
        }

        /* Otherwise, success. Show the loaded message for 2 seconds and inform. */
        labelRef.value = "Loaded!"
        setTimeout(() => {
            labelRef.value = props.label
        }, 2000)

        emit("success", response)
    } catch(e: unknown) {
        /* Be sure the label is reset if anything goes wrong... */
        labelRef.value = props.label
    }

}
</script>