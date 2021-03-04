<template>
    <v-layout align-center justify-center>
         <v-flex xs12 sm4 md4 lg4 x14>
             <v-card>
                 <v-toolbar dark color="blue darken-3">
                      <v-icon>perm_identity</v-icon>
                    <v-toolbar-title>
                        Acceso al Sistema
                    </v-toolbar-title>
                </v-toolbar>
                <v-card-text>
                    <v-form ref="loginForm">
                        <v-tooltip right content-class="subheading">
                            <template v-slot:activator="{ on }">  
                                <v-text-field v-on="on" v-model="email" prepend-icon="email"  color="accent" label="Email" :rules="emailRules" >
                                </v-text-field>
                            </template>
                            <span><strong>¡Bienvenido!</strong></span>
                            <p><strong>Usa algunas de las siguientes cuentas para comenzar:</strong></p>
                            <p><ul>
                                <li>vendedor@gmail.com <strong>password:</strong>1234</li>
                                <li>almacenero@gmail.com <strong>password:</strong>1234</li>
                                <li>admin@gmail.com <strong>password:</strong>1234</li>
                            </ul></p>
                        </v-tooltip>

                        <v-text-field v-model="password" prepend-icon="vpn_key" :type="show1 ? 'text' : 'password'" color="accent" label="Password"
                        :append-icon="show1 ? 'visibility' : 'visibility_off'" 
                        @click:append="show1 = !show1"
                        :rules="inputRules">
                        </v-text-field>
                        <v-flex v-if="error" class="red--text">
                            {{error}}
                        </v-flex>
                    </v-form>                    
                </v-card-text>
                <v-card-actions class="px-3 pb-3">
                    <v-flex text-xs-right>
                        <v-btn @click="ingresar" color="primary" :loading="loading">Ingresar</v-btn>
                    </v-flex>
                </v-card-actions>
            </v-card>
        </v-flex>
    </v-layout>

</template>
<script>
import axios from 'axios'
export default{
    data(){
        return{
            email:'',
            password:'',
            error: null,
            loading: false,
            show1: false,
            inputRules:[
                v => !!v || 'El campo es requerido',
            ],
            emailRules:[
                v => !!v || 'El email es requerido',
                v => /.+@.+\..+/.test(v) || 'No es un email válido'
            ]
        }
    },
    

    created(){
       this.logueado();
  },

    methods:{
        ingresar(){
            if (this.$refs.loginForm.validate()) {
                this.loading= true;
                this.error=null;
                axios.post('api/Usuarios/Login', {email: this.email, password: this.password})
                .then(respuesta =>{
                    return respuesta.data
                })
                .then(data =>{
                    this.$store.dispatch("guardarToken", data.token)
                    this.$router.push({name: 'home'})
                })
                .catch(err =>{
                    if(err.response.status==404){
                        this.error="No existe el usuario o sus datos son incorrectos";
                    }else{
                        this.error="Ocurrió un error"
                    }
                    //console.log(err)
                    this.loading= false;
                })
            }
            
        },

        logueado(){
             if (this.$store.state.usuario) {
            this.$router.push({name: 'home'})
          }
        }
    }

    

}
</script>