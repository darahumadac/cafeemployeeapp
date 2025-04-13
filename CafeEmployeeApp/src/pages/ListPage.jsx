import React, { useEffect, useState } from "react";
import { Button, Container } from "@mui/material";
import Table from "../Table.jsx";
import axios from "axios";
import { useLocation } from "react-router-dom";
import { API_URL, DATA_HEADERS } from "../../config.js";
import { AgGridReact } from "ag-grid-react";
import "ag-grid-community/styles/ag-grid.css";
import "ag-grid-community/styles/ag-theme-alpine.css";
import ConfirmModal from "../ConfirmModal.jsx";

const ButtonCellRenderer = ({ data, onDelete }) => {
  const { pathname } = useLocation();
  return (
    <>
      <Button href={`${pathname}/${data.id}`}>Edit</Button>
      <Button
        onClick={(e) => onDelete(e, data)}
        href={`${API_URL}${pathname.slice(0, -1)}/${data.id}`}
      >
        Delete
      </Button>
    </>
  );
};

const ListPage = () => {
  const [loading, setLoading] = useState(true);
  const [isOpen, setIsOpen] = useState(false);
  const [reload, setReload] = useState(true);
  const [lastModified, setLastModified] = useState("");

  const handleDelete = (e, data) => {
    e.preventDefault();

    console.log(e);
    console.log(e.target.href);
    console.log(data);
    // console.log(e);
    const toDelete = confirm(
      `are you sure you want to delete: ${data.name} - ${data.location}`
    );
    if (toDelete) {
      // setLoading(true);
      axios
        .delete(e.target.href)
        .then((response) => {
          console.log("reloading");
          setLastModified(response.headers["last-modified"]);
          setReload(true);
        })
        .catch(() => {
          console.log("error deleting");
        })
        .finally(() => {
          console.log("done delete refresh");
          // setLoading(false);
        });
    }

    //show modal confirm
    //if yes, call
  };

  const { pathname } = useLocation();
  const columns = DATA_HEADERS[pathname]
    .map((h) => ({ field: h }))
    .concat([
      {
        field: "",
        cellRenderer: "ButtonCellRenderer",
        cellRendererParams: { onDelete: handleDelete },
      },
    ]);

  const [rows, setRows] = useState([]);

  useEffect(() => {
    if (!reload) return;
    console.log("reloading table");
    setLoading(true);
    axios.get(`${API_URL}${pathname}`, {
      headers: {
        "If-Modified-Since": lastModified,
        "Cache-Control": "no-store"
      }
    })
      .then((response) => {
        const dataList = response.data;
        console.log(dataList);
        setLastModified(response.headers["last-modified"]);
        // console.log(response.headers);
        // setLoading(false);
        setRows(dataList);
      })
      .catch((err) => {
        console.log(err);
      })
      .finally(() => {
        // setLoading(false);
        setReload(false);
      });
  }, [reload]);

  return (
    <>
      {/* <ConfirmModal open={isOpen} handleClose={() => setIsOpen(false)} /> */}
      <Container maxWidth style={{ height: 400 }} className="ag-theme-material">
        <Button onClick={() => setReload(true)}>Refresh</Button>
        <AgGridReact
          rowData={rows}
          columnDefs={columns}
          pagination={true}
          components={{ ButtonCellRenderer }}
        />
      </Container>
    </>
  );
};

export default ListPage;
