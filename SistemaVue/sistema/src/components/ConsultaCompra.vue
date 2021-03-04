<template>
    <v-layout align-start>
        <v-flex>
            <v-card>
                <v-card-title>
                        <v-flex xs12 sm2 md2 lg2 text-xs-center>
                            <h2>Consulta Compras</h2>                             
                        </v-flex>
                        <v-divider
                    vertical
                    ></v-divider>
                   
                    <!-- Datepicker -->
                        <v-flex xs8 sm3 md2 text-xs-center v-if="verNuevo==0">
                            <v-menu
                            v-model="menu1"
                            full-width
                            max-width="290"
                            lazy
                            transition="scale-transition"
                            :close-on-content-click="false"
                            >
                                <template v-slot:activator="{ on }">
                                    <v-text-field 
                                     readonly
                                     class="text-xs-center" 
                                     label="Desde:" 
                                     prepend-icon="event" 
                                     clearable 
                                     clear-icon="highlight_off" 
                                     v-on="on"
                                     :value="fechaInicioFormateada"
                                    @click:clear="fechaInicio=''"></v-text-field>
                                </template>
                                <v-date-picker
                                v-model="fechaInicio"
                                locale="es"
                                @change="menu1 = false"
                                ></v-date-picker>
                            </v-menu>
                        </v-flex>
                    <v-flex xs8 sm3 md2 text-xs-center v-if="verNuevo==0">
                        <v-menu
                            v-model="menu2"
                            full-width
                            max-width="290"
                            lazy
                            transition="scale-transition"
                            :close-on-content-click="false"
                            >
                             <template v-slot:activator="{ on }">
                                <v-text-field 
                                readonly
                                class="text-xs-center" 
                                label="Hasta:"  
                                prepend-icon="event" 
                                clearable 
                                clear-icon="highlight_off" 
                                v-on="on" 
                                :value="fechaFinFormateada"
                                @click:clear="fechaFin=''"></v-text-field>
                             </template>
                            <v-date-picker
                                v-model="fechaFin"
                                locale="es"
                                @change="menu2 = false"
                                ></v-date-picker>    
                        </v-menu> 
                    </v-flex>
                    
                    <v-flex xs12 sm3 text-xs-center>
                    <v-btn v-if="verNuevo==0" @click="listar();loadingBtnBuscar=true" :loading="loadingBtnBuscar" color="primary" dark >Buscar</v-btn> 
                    </v-flex>
                </v-card-title>
            <v-data-table
                :headers="headers"
                :items="ingresos"
                :search="search"
                :loading="loading"
                :pagination.sync="pagination"
                class="elevation-1"
                v-if="verNuevo==0"
            >
                <template v-slot:items="props">
                    <td class="justify-center layout px-0">
                        <v-icon
                        small
                        class="mr-2"
                        @click="verDetalles(props.item)"
                        >
                        tab
                        </v-icon>
                    </td>
                    <td>{{ props.item.usuario }}</td>
                    <td>{{ props.item.proveedor }}</td>
                    <td>{{ props.item.tipo_comprobante }}</td>
                    <td>{{ props.item.serie_comprobante }}</td>
                    <td>{{ props.item.num_comprobante }}</td>
                    <td>{{ props.item.fecha_hora }}</td>
                    <td>%{{ props.item.impuesto }}</td>
                    <td>${{ props.item.total }}</td>
                    <td>
                        <div v-if="props.item.estado=='Aceptado'">
                            <span class="blue--text">Aceptado</span>
                        </div>
                        <div v-else>
                            <span class="red--text">{{props.item.estado}}</span>
                        </div>
                    </td>
                
                </template>
                <template v-slot:no-data>
                <v-btn color="primary" @click="listar">Resetear</v-btn>
                </template>
            </v-data-table>
            <v-slide-y-transition mode="out-in">
            <template>
            <v-container grid-list-sm class="pa-4 white" v-if="verNuevo">
                <v-form ref="form">
                <v-layout row wrap >
                    
                        <v-flex xs12 sm4 md4 lg4 xl4>
                            <v-select v-model="tipo_comprobante"
                            :items="comprobantes" label="Tipo Comprobante" :rules="[v => !!v || 'Seleccione un Tipo Comprobante']" readonly>
                            </v-select>
                        </v-flex>
                        <v-flex xs12 sm4 md4 lg4 xl4>
                            <v-text-field v-model="serie_comprobante" label="Serie Comprobante" :rules="inputRules" readonly>
                            </v-text-field>
                        </v-flex>
                        <v-flex xs12 sm4 md4 lg4 xl4>
                            <v-text-field v-model="num_comprobante" label="Número Comprobante" :rules="inputRules" readonly>
                            </v-text-field>
                        </v-flex>
                        <v-flex xs12 sm8 md8 lg8 xl8>
                            <v-select v-model="idproveedor"
                            :items="proveedores" label="Proveedor" :rules="[v => !!v || 'Seleccione un Proveedor']" readonly>
                            </v-select>
                        </v-flex>
                        <v-flex xs12 sm4 md4 lg4 xl4>
                            <v-text-field type="number" v-model="impuesto" label="Impuesto" 
                            :rules="[v => !!v || 'El campo es requerido',
                                     v => v>0 ||'Ingrese un impuesto válido']" readonly>
                            </v-text-field>
                        </v-flex>
                       
                    <v-flex xs12 sm2 md2 lg2 xl2 v-if="errorArticulo">
                        <div class="red--text" v-text="errorArticulo">
                        </div>
                    </v-flex>
                    <v-flex xs12 sm12 md12 lg12 xl12>
                        <v-data-table
                            :headers="cabeceraDetalles"
                            :items="detalles"
                            :loading="loading"
                            hide-actions
                            class="elevation-1"
                        >
                            <template v-slot:items="props">
                                <td>{{ props.item.articulo }}</td>
                                <td><v-text-field type="number" v-model="props.item.cantidad" readonly></v-text-field></td>
                                <td><v-text-field type="number" prefix="$" v-model="props.item.precio" readonly></v-text-field></td>
                                <td>$ {{ props.item.cantidad * props.item.precio}}</td>
                            </template>
                            <template v-slot:no-data>
                                <h3>No hay artículos agregados al detalle</h3>
                            </template>
                        </v-data-table>
                        <v-flex class="text-xs-right">
                            <strong>Total Parcial: </strong>$ {{totalParcial=(total-totalImpuesto).toFixed(2)}}
                        </v-flex>
                        <v-flex class="text-xs-right">
                            <strong>Total Impuesto: </strong>$ {{totalImpuesto=((total*impuesto)/(100+impuesto)).toFixed(2)}}
                        </v-flex>
                        <v-flex class="text-xs-right">
                            <strong>Total Neto: </strong>$ {{total=(calcularTotal).toFixed(2)}}
                        </v-flex>
                    </v-flex>
                    <v-flex xs12 sm12 md12 v-show="valida">
                        <div class="red--text" v-for="v in validaMensaje" :key="v" v-text="v">
                        </div>
                     </v-flex>
                    <v-flex xs12 sm12 md12 lg12 xl12>
                        <v-btn @click="ocultarNuevo" color="blue darken-1" flat>Volver</v-btn>
                        <v-btn v-if="verDet==0" @click="guardar" color="success" :loading="loadingBtn">Guardar</v-btn>
                    </v-flex>
                </v-layout>
                </v-form>
            </v-container>
            </template>
            </v-slide-y-transition>
            </v-card>
        </v-flex>
    </v-layout>
</template>

<script>
    import axios from 'axios'
    import moment from 'moment'
    export default {
        data(){
            return{

                pagination: {
                    sortBy: 'fecha_hora',
                    descending: true
                },

                ingresos:[],
                dialog: false,
                headers: [
                { text: 'Opciones', value: 'opciones', sortable: false },
                { text: 'Usuario', value: 'usuario' },
                { text: 'Proveedor', value: 'proveedor' },
                { text: 'Tipo Comprobante', value: 'tipo_comprobante' },
                { text: 'Serie Comprobante', value: 'serie_comprobante',sortable: false},
                { text: 'Número Comprobante', value: 'num_comprobante',sortable: false},
                { text: 'Fecha', value: 'fecha_hora'},
                { text: 'Impuesto', value: 'impuesto',sortable: false},
                { text: 'Total', value: 'total',sortable: false},
                { text: 'Estado', value: 'estado', sortable: false }
                
                ],
                cabeceraDetalles: [
                { text: 'Artículo', value: 'articulo', sortable: false },
                { text: 'Cantidad', value: 'cantidad',sortable: false},
                { text: 'Precio', value: 'precio',sortable: false},
                { text: 'Subtotal', value: 'subtotal', sortable: false }
                ],

                cabeceraArticulos:[
                    { text: 'Seleccionar', value: 'seleccionar', sortable: false },
                    { text: 'Artículo', value: 'articulo' },
                    { text: 'Categoría', value: 'categoria'},
                    { text: 'Descripción', value: 'descripcio',sortable: false},
                    { text: 'Stock', value: 'stock', sortable: false },
                    { text: 'Precio Venta', value: 'precio_venta',sortable: false},

                ],

                detalles:[],

                search: '',
                loading: true,
                loadingBtn: false,
                loadingBtnBuscar: false,
                id: '',
                idproveedor:'',
                proveedores:[],
                tipo_comprobante:'',
                comprobantes:['FACTURA','BOLETA','TICKET'],
                serie_comprobante: '',
                num_comprobante: '',
                impuesto: 18,
                codigo:'',
                verNuevo:0,
                errorArticulo: null,
                totalParcial:0,
                totalImpuesto:0,
                total:0,
                articulos:[],
                texto:'',
                verArticulos:0,
                verDet:0,
                valida: 0,
                validaMensaje:[],
                adModal: 0,
                adAccion: 0,
                adNombre: '',
                adId: '',
                inputRules:[
                     v => !!v || 'El campo es requerido',
                ],

                //Variables para datepickers
                menu1: false,
                menu2: false,
                fechaInicio: '',
                fechaFin:'',


            }

        },
            computed: {
                calcularTotal:function(){
                    var resultado=0.0;
                    for (var i =0;i <this.detalles.length; i++) {
                        resultado=resultado+(this.detalles[i].precio*this.detalles[i].cantidad);
                    }
                    return resultado;
                },

                 fechaInicioFormateada () {
                    return this.fechaInicio ? moment(this.fechaInicio).format('DD/MM/YYYY'):''
                },

                
                  fechaFinFormateada () {
                    return this.fechaFin ? moment(this.fechaFin).format('DD/MM/YYYY'):''
                }

               

            },

            watch: {
                dialog (val) {
                val || this.close()
                }
            },

            created () {
                this.listar();
                this.select();
            },
        methods:{

             
            mostrarNuevo(){
                this.verNuevo=1;
            },
            ocultarNuevo(){
                this.verNuevo=0;
                this.limpiar();
            },

            buscarCodigo(){
                let me=this;
                me.errorArticulo=null;
                let header={"Authorization" : "Bearer " + this.$store.state.token};
                let configuracion={headers : header};
                axios.get('api/Articulos/BuscarCodigoIngreso/'+this.codigo,configuracion).then(function(response){
                    //console.log(response);
                    me.agregarDetalle(response.data);
                    me.loading=false;

                }).catch(function(error){
                   // console.log(error);
                    me.errorArticulo='No existe el artículo';
                });

            },

            listarArticulo(){
                let me=this;
                let header={"Authorization" : "Bearer " + this.$store.state.token};
                let configuracion={headers : header};
                axios.get('api/Articulos/ListarIngreso/'+me.texto,configuracion).then(function(response){
                    //console.log(response);
                    me.articulos=response.data;
                    //me.loading=false;

                }).catch(function(error){
                    console.log(error);
                });
            },

            mostrarArticulos(){
                this.verArticulos=1;
            },
            ocultarArticulos(){
                this.verArticulos=0;
            },

            agregarDetalle(data=[]){
                this.errorArticulo=null;
                if (this.encuentra(data['idarticulo'])) {
                    this.errorArticulo="El artículo ya ha sido agregado";
                } else {
                    this.detalles.push(
                    {idarticulo:data['idarticulo'],
                    articulo: data['nombre'],
                    cantidad:1,
                    precio:1}
                  );
                }
                
            },

            encuentra(id){
                var sw=0;
                for (var i = 0; i<this.detalles.length; i++) {
                    if (this.detalles[i].idarticulo==id) {
                        sw=1;
                    }
                }
                return sw;
            },

            eliminarDetalle(arr,item){
                var i= arr.indexOf(item);
                if (i!==-1) {
                    arr.splice(i,1);
                }
            },

            listar(){
                let me=this;
                let header={"Authorization" : "Bearer " + this.$store.state.token};
                let configuracion={headers : header};
                let url='';
                if (!me.fechaInicio || !me.fechaFin) {
                    url='api/Ingresos/Listar';
                } else {
                    url='api/Ingresos/ConsultaFechas/'+me.fechaInicio+'/'+me.fechaFin;
                }
                axios.get(url,configuracion).then(function(response){
                    //console.log(response);
                    me.ingresos=response.data;
                    //Formateo de fecha y hora
                    me.ingresos.map(function(x){
                        x.fecha_hora=moment(x.fecha_hora).format('DD/MM/YYYY HH:mm');
                    });
                    me.loading=false;
                    me.loadingBtnBuscar=false;

                }).catch(function(error){
                    console.log(error);
                });

            },

            
            listarDetalles(id){
                let me=this;
                let header={"Authorization" : "Bearer " + this.$store.state.token};
                let configuracion={headers : header};
                axios.get('api/Ingresos/ListarDetalles/'+id,configuracion).then(function(response){
                    //console.log(response);
                    me.detalles=response.data;
                    me.loading=false;

                }).catch(function(error){
                    console.log(error);
                });

            },

            verDetalles(item){
                this.limpiar();
                this.tipo_comprobante=item.tipo_comprobante;
                this.serie_comprobante=item.serie_comprobante;
                this.num_comprobante=item.num_comprobante;
                this.idproveedor=item.idproveedor;
                this.impuesto=item.impuesto;
                this.listarDetalles(item.idingreso);
                this.verNuevo=1;
                this.verDet=1;
            },

            select(){
                let me=this;
                let header={"Authorization" : "Bearer " + this.$store.state.token};
                let configuracion={headers : header};
                var proveedoresArray=[];
                axios.get('api/Personas/SelectProveedores',configuracion).then(function(response){
                    proveedoresArray=response.data;
                    proveedoresArray.map(function(x){
                        me.proveedores.push({text: x.nombre, value:x.idpersona});
                    });

                }).catch(function(error){
                    console.log(error);
                });

            },

            limpiar(){
                this.id="";
                this.idproveedor="";
                this.tipo_comprobante="";
                this.serie_comprobante="";
                this.num_comprobante="";
                this.impuesto="18";
                this.codigo="";
                this.detalles=[];
                this.total=0;
                this.totalImpuesto=0
                this.totalParcial=0;
                this.verDet=0;
                this.loadingBtn=false;
                this.validaMensaje=[];
            },

            guardar () {

                    if (this.$refs.form.validate()) {
                        if (this.validar()) {
                            return;               
                        }

                        let header={"Authorization" : "Bearer " + this.$store.state.token};
                        let configuracion={headers : header};
                            //Código para guardar
                            let me=this;
                            me.loadingBtn=true;
                            axios.post('api/Ingresos/Crear',{
                                'idproveedor':me.idproveedor,
                                'idusuario':me.$store.state.usuario.idusuario,
                                'tipo_comprobante': me.tipo_comprobante,
                                'serie_comprobante': me.serie_comprobante,
                                'num_comprobante': me.num_comprobante,
                                'impuesto': me.impuesto,
                                'total': me.total,
                                'detalles':me.detalles
                            },configuracion).then(function(response){
                                me.ocultarNuevo();
                                me.listar();
                                me.limpiar();
                                me.$emit('registroGuardado');
                            }).catch(function(error){
                                console.log(error);
                            });     
                        
                    }
                },

                validar(){
                this.valida=0;
                this.validaMensaje=[];
                
                if (this.detalles.length<=0) {
                    this.validaMensaje.push("Ingrese al menos un artículo al detalle.");
                }

                if(this.validaMensaje.length){
                    this.valida=1;
                }

                return this.valida;

            },

            activarDesactivarMostrar(accion,item){
                this.adModal=1;
                this.adNombre= item.num_comprobante;
                this.adId=item.idingreso;


                if (accion==1) {
                    this.adAccion=1;
                }
                else if(accion=2){
                    this.adAccion=2;

                }
                else{
                    this.adModal=0;
                }
            },

            activarDesactivarCerrar(){
                     this.adModal=0;
            },
            /*
            activar(){
                    let me=this;
                    let header={"Authorization" : "Bearer " + this.$store.state.token};
                    let configuracion={headers : header};
                            axios.put('api/Usuarios/Activar/'+this.adId,{},configuracion).then(function(response){
                                me.adModal=0;
                                me.adAccion=0;
                                me.adNombre="";
                                me.adId="";
                                me.listar();
                            }).catch(function(error){
                                console.log(error);
                            });
            },
            */
            desactivar(){
                    let me=this;
                    let header={"Authorization" : "Bearer " + this.$store.state.token};
                    let configuracion={headers : header};
                            axios.put('api/Ingresos/Anular/'+this.adId,{},configuracion).then(function(response){
                                me.adModal=0;
                                me.adAccion=0;
                                me.adNombre="";
                                me.adId="";
                                me.listar();
                            }).catch(function(error){
                                console.log(error);
                            });

            }

        }
            
    }
</script>
