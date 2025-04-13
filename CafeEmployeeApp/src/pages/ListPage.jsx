import React, { useEffect, useState, useRef, useCallback } from "react";
import { Button, Container } from "@mui/material";
import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TablePagination,
  Paper,
} from "@mui/material";
// import Table from "../Table.jsx";
import axios from "axios";
import { Link, useLocation } from "react-router-dom";
import { API_URL, DATA_HEADERS } from "../../config.js";
import { AgGridReact } from "ag-grid-react";
import "ag-grid-community/styles/ag-grid.css";
import "ag-grid-community/styles/ag-theme-alpine.css";
import ConfirmModal from "../ConfirmModal.jsx";
import SearchBar from "../SearchBar.jsx";

const ListPage = () => {
  const [rows, setRows] = useState([]);

  useEffect(() => {
    loadData();
  }, []);

  const [page, setPage] = useState(0); // Current page
  const [rowsPerPage, setRowsPerPage] = useState(5); // Rows per page

  // Pagination handlers
  const handleChangePage = (event, newPage) => setPage(newPage);
  const handleChangeRowsPerPage = (event) => {
    setRowsPerPage(parseInt(event.target.value, 10));
    setPage(0); // Reset to first page
  };

  const loadData = () => {
    console.log(`${API_URL}${pathname}`);
    setLoading(true);
    axios
      .get(`${API_URL}${pathname}`, {
        headers: {
          "If-Modified-Since": lastModified,
        },
      })
      .then((response) => {
        const dataList = response.data;
        setLastModified(response.headers["last-modified"]);
        setRows(dataList);
        console.log("done set data row");
      })
      .catch((err) => {
        console.log(err);
      })
      .finally(() => {
        setLoading(false);
      });
  };

  const onRefreshClick = useCallback(() => {
    loadData();
  }, []);

  const [confirmDeleteOpen, setConfirmDeleteOpen] = useState(false);

  const handleDelete = (e, data) => {
    e.preventDefault();
    setConfirmDeleteOpen(true);
    const toDelete = confirm(`are you sure you want to delete: ${data.name}`);
    if (toDelete) {
      axios
        .delete(e.target.href)
        .then((response) => {
          console.log("reloading");
          setLastModified(response.headers["last-modified"]);
          loadData();
        })
        .catch(() => {
          // console.log("error deleting");
        })
        .finally(() => {
          // console.log("done delete refresh");
        });
    }

    //show modal confirm
    //if yes, call
  };

  const { pathname } = useLocation();
  const [loading, setLoading] = useState(true);
  const [reload, setReload] = useState(true);
  const [lastModified, setLastModified] = useState("");

  const [columns] = useState(DATA_HEADERS[pathname].map((h) => ({ field: h })));

  // Get visible rows
  const paginatedRows = rows.slice(
    page * rowsPerPage,
    page * rowsPerPage + rowsPerPage
  );

  return (
    <>
      {/* <ConfirmModal open={confirmDeleteOpen} /> */}
      <Paper>
        <Button onClick={onRefreshClick}>Refresh</Button>
        <SearchBar toSearch={pathname.slice(1, -1)}/>
        <TableContainer>
          <Table>
            <TableHead>
              <TableRow>
                {columns.map((c) => (
                  <TableCell key={c.field}>{c.field.toUpperCase()}</TableCell>
                ))}
                <TableCell></TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {paginatedRows.map((row) => (
                <TableRow key={row.id}>
                  {columns.map((c) => (
                    <TableCell key={c.field}>
                      {c.field == "employees" && 
                        <Link to={{
                          pathname: "/employees",
                          search: `?cafe=${row.id}`
                        }}>{row[c.field]}</Link> 
                        || row[c.field] }
                      
                    </TableCell>
                  ))}
                  <TableCell>
                    <Button href={`${pathname}/${row.id}`}>Edit</Button>
                    <Button
                      onClick={(e) => handleDelete(e, row)}
                      href={`${API_URL}${pathname.slice(0, -1)}/${row.id}`}
                    >
                      Delete
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>

        {/* foorter */}
        <TablePagination
          component="div"
          count={rows.length}
          page={page}
          onPageChange={handleChangePage}
          rowsPerPage={rowsPerPage}
          onRowsPerPageChange={handleChangeRowsPerPage}
          rowsPerPageOptions={[5, 10, 25]}
        />
      </Paper>
    </>
  );
};

export default ListPage;
