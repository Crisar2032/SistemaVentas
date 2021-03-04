<template>
    <v-layout align-start>
        <v-flex>
            <v-toolbar flat color="white">
                <v-toolbar-title>Clientes</v-toolbar-title>
                    <v-divider
                    class="mx-2"
                    inset
                    vertical
                    ></v-divider>
                    <v-spacer></v-spacer>
                    <v-text-field class="text-xs-center" v-model="search" append-icon="search" label="Búsqueda" single-line hide-details></v-text-field>
                    <v-spacer></v-spacer>
                    <v-dialog v-model="dialog" max-width="500px">
                        <v-btn slot="activator" color="primary" dark class="mb-2">Nuevo</v-btn>
                        <v-form ref="form">
                            <v-card>
                                <v-card-title>
                                <span class="headline">{{ formTitle }}</span>
                                </v-card-title>
                    
                                <v-card-text>
                                <v-container grid-list-md>
                                    <v-layout wrap>
                                    <v-flex xs12 sm12 md12>
                                        <v-text-field v-model="nombre" label="Nombre" :rules="nombreRules"></v-text-field>
                                    </v-flex>
                                    <v-flex xs12 sm6 md6>
                                        <v-select v-model="tipo_documento" :items="documentos" label="Tipo Documento" :rules="inputRules">
                                        </v-select>
                                    </v-flex>
                                    <v-flex xs12 sm6 md6>
                                        <v-text-field v-model="num_documento" label="Número Documento" maxlength="20" :rules="inputRules"></v-text-field>
                                    </v-flex>
                                    <v-flex xs12 sm12 md12>
                                        <v-text-field v-model="direccion" label="Dirección" maxlength="70" :rules="inputRules"></v-text-field>
                                    </v-flex>
                                    <v-flex xs12 sm6 md6>
                                        <v-text-field v-model="telefono" label="Teléfono" maxlength="20" :rules="inputRules"></v-text-field>
                                    </v-flex>
                                    <v-flex xs12 sm6 md6>
                                        <v-text-field v-model="email" label="Email" :rules="emailRules"></v-text-field>
                                    </v-flex>
                                    </v-layout>
                                </v-container>
                                </v-card-text>
                    
                                <v-card-actions>
                                <v-spacer></v-spacer>
                                <v-btn color="blue darken-1" flat @click="close">Cancelar</v-btn>
                                <v-btn color="blue darken-1" flat @click="guardar" :loading="loadingBtn">Guardar</v-btn>
                                </v-card-actions>
                            </v-card>
                        </v-form>
                    </v-dialog>

            </v-toolbar>
            <v-data-table
                :headers="headers"
                :items="clientes"
                :search="search"
                :loading="loading"
                class="elevation-1"
            >
                <template v-slot:items="props">
                    <td class="justify-center layout px-0">
                        <v-icon
                        small
                        class="mr-2"
                        @click="editItem(props.item)"
                        >
                        edit
                        </v-icon>
                    </td>
                    <td>{{ props.item.nombre }}</td>
                    <td>{{ props.item.tipo_persona }}</td>
                    <td>{{ props.item.tipo_documento }}</td>
                    <td>{{ props.item.num_documento }}</td>
                    <td>{{ props.item.direccion }}</td>
                    <td>{{ props.item.telefono }}</td>
                    <td>{{ props.item.email }}</td>
                
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

                clientes:[],
                dialog: false,
                headers: [
                { text: 'Opciones', value: 'opciones', sortable: false },
                { text: 'Nombre', value: 'nombre' },
                { text: 'Tipo Persona', value: 'tipo_persona' },
                { text: 'Tipo Documento', value: 'tipo_documento' },
                { text: 'Número Documento', value: 'num_documento',sortable: false},
                { text: 'Dirección', value: 'direccion',sortable: false},
                { text: 'Teléfono', value: 'telefono',sortable: false},
                { text: 'Email', value: 'email',sortable: false}
                
                ],
                search: '',
                editedIndex: -1,
                loading: true,
                loadingBtn: false,
                id: '',
                nombre:'',
                tipo_documento:'',
                documentos:['DNI','RUC','PASAPORTE','CEDULA'],
                num_documento: '',
                direccion: '',
                telefono: '',
                email: '',
                adModal: 0,
                adAccion: 0,
                adNombre: '',
                adId: '',

                nombreRules:[
                v => !!v || 'El campo es requerido',
                v => v && v.length > 3 && v.length <=100 || 'El nombre debe tener más de 3 caracteres y menos de 100 caracteres'
                ],
                emailRules:[
                v => !!v || 'El email es requerido',
                v => /.+@.+\..+/.test(v) || 'No es un email válido'
                ],
                inputRules:[v => !!v || 'El campo es requerido']


            }

        },
            computed: {
                formTitle () {
                return this.editedIndex === -1 ? 'Nuevo cliente' : 'Actualizar cliente'
                }
            },

            watch: {
                dialog (val) {
                val || this.close()
                }
            },

            created () {
                this.listar();
            },
        methods:{
            listar(){
                let me=this;
                let header={"Authorization" : "Bearer " + this.$store.state.token};
                let configuracion={headers : header};
                axios.get('api/Personas/ListarClientes',configuracion).then(function(response){
                    //console.log(response);
                    me.clientes=response.data;
                    me.loading=false;

                }).catch(function(error){
                    console.log(error);
                });

            },


            editItem (item) {
                this.id=item.idpersona;
                this.nombre=item.nombre;
                this.tipo_documento= item.tipo_documento;
                this.num_documento=item.num_documento;
                this.direccion=item.direccion;
                this.telefono = item.telefono;
                this.email = item.email;
                this.editedIndex=1;
                this.dialog = true
            },

            close () {
                this.dialog = false;
                this.limpiar();
            },

            limpiar(){
                this.id="";
                this.nombre="";
                this.tipo_documento="";
                this.num_documento="";
                this.direccion="";
                this.telefono="";
                this.email="";
                this.loadingBtn=false;
                this.$refs.form.reset();
                this.editedIndex=-1;
            },

            guardar () {

                    if (this.$refs.form.validate()) {
                                       
                    
                        let header={"Authorization" : "Bearer " + this.$store.state.token};
                        let configuracion={headers : header};

                        if (this.editedIndex > -1) {
                            //Código para editar                        
                            let me=this;
                            me.loadingBtn=true;
                            axios.put('api/Personas/Actualizar',{
                                'idpersona':me.id,
                                'tipo_persona':'Cliente',
                                'nombre':me.nombre,
                                'tipo_documento': me.tipo_documento,
                                'num_documento': me.num_documento,
                                'direccion': me.direccion,
                                'telefono': me.telefono,
                                'email': me.email
                            },configuracion).then(function(response){
                                me.close();
                                me.listar();
                                me.limpiar();
                            }).catch(function(error){
                                console.log(error);
                            });
                        
                        } else {
                            //Código para guardar
                            let me=this;
                            me.loadingBtn=true;
                            axios.post('api/Personas/Crear',{
                                'tipo_persona':'Cliente',
                                'nombre':me.nombre,
                                'tipo_documento': me.tipo_documento,
                                'num_documento': me.num_documento,
                                'direccion': me.direccion,
                                'telefono': me.telefono,
                                'email': me.email
                            },configuracion).then(function(response){
                                me.close();
                                me.listar();
                                me.limpiar();
                                me.$emit('registroGuardado');
                            }).catch(function(error){
                                console.log(error);
                            });
                            
                        }
                    }
                },

                

           

        }
            
    }
</script>
