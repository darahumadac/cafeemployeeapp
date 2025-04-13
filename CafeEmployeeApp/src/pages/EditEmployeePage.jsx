import React, {useEffect, useState} from "react";
import { Container } from "@mui/material";
import Table from "../Table";
import axios from "axios";
import { useParams } from "react-router-dom";



const EditEmployeePage = () => 
{
    const {id} = useParams();
    return (<div>edit employee page {id}</div>)

}

export default EditEmployeePage;