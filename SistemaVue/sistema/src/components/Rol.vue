<template>
    <v-layout align-start>
        <v-flex>
            <v-toolbar flat color="white">
                <v-toolbar-title>Roles</v-toolbar-title>
                    <v-divider
                    class="mx-2"
                    inset
                    vertical
                    ></v-divider>
                    <v-spacer></v-spacer>
                    <v-text-field class="text-xs-center" v-model="search" append-icon="search" label="Búsqueda" single-line hide-details></v-text-field>
                    <v-spacer></v-spacer>

            </v-toolbar>
            <v-data-table
                :headers="headers"
                :items="roles"
                :search="search"
                :loading="loading"
                class="elevation-1"
            >
                <template v-slot:items="props">
                    <td>{{ props.item.nombre }}</td>
                    <td>{{ props.item.descripcion }}</td>
                    <td>
                        <div v-if="props.item.condicion">
                            <span class="blue--text">Activo</span>
                        </div>
                        <div v-else>
                            <span class="red--text">Inactivo</span>
                        </div>
                    </td>
                
                </template>
                <template v-slot:no-data>
                <v-btn color="primary" @click="listar">Resetear</v-btn>
                </template>
            </v-data-table>
        </v-flex>
    </v-layout>
</template>

<script>
    import axios from 'axios'
    export default {
        data(){
            return{

                roles:[],
                dialog: false,
                headers: [
                { text: 'Nombre', value: 'nombre' },
                { text: 'Descripción', value: 'descripcion',sortable: false},
                { text: 'Estado', value: 'condicion', sortable: false },
                
                ],
                search: '',
                loading: true,

            }

        },
            computed: {
            },

            watch: {
            },

            created () {
                this.listar();
            },
        methods:{
            listar(){
                let me=this;
                let header={"Authorization" : "Bearer " + this.$store.state.token};
                let configuracion={headers : header};
                axios.get('api/Roles/Listar',configuracion).then(function(response){
                    //console.log(response);
                    me.roles=response.data;
                    me.loading=false;

                }).catch(function(error){
                    console.log(error);
                });

            }

        }
            
    }
</script>
